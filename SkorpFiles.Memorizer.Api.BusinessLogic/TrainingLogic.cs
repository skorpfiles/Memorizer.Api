using SkorpFiles.Memorizer.Api.BusinessLogic.Training;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public class TrainingLogic:ITrainingLogic
    {
        private readonly ITrainingRepository _trainingRepository;

        public TrainingLogic(ITrainingRepository trainingRepository)
        {
            _trainingRepository = trainingRepository;
        }

        public async Task<IEnumerable<Api.Models.Question>> SelectQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingOptions options)
        {
            if (options.NewQuestionsFraction < 0 || options.PrioritizedPenaltyQuestionsFraction < 0)
                throw new IncorrectTrainingOptionsException(Constants.NegativeFractionsMessage);

            if (options.NewQuestionsFraction + options.PrioritizedPenaltyQuestionsFraction > 1)
                throw new IncorrectTrainingOptionsException(Constants.SumOfFractionsCannotBeMoreThan1Message);

            if (options.LengthValue <= 0)
                throw new IncorrectTrainingOptionsException(Constants.NonPositiveLengthValueMessage);

            var allQuestions = (await _trainingRepository.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ToList();
            var questionsListsCollection = new TrainingBuilder(allQuestions);
            var questionsList = questionsListsCollection.MakeQuestionsListForTraining(options);

            return questionsList;
        }

        public async Task<UserQuestionStatus> UpdateQuestionStatusAsync(Guid userId, TrainingResultRequest requestData)
        {
            if (requestData == null) 
                throw new ArgumentNullException(nameof(requestData));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            if (requestData.QuestionId == Guid.Empty)
                throw new ArgumentException("QuestionId cannot be empty.");

            if (requestData.AnswerTimeMilliseconds <= 0)
                throw new ArgumentException("AnswerTime must be positive.");

            var currentUserQuestionStatus = await _trainingRepository.GetUserQuestionStatusAsync(userId, requestData.QuestionId);
            currentUserQuestionStatus ??= CreateNewUserQuestionStatus(userId, requestData.QuestionId);

            UpdateUserQuestionStatusByAnswer(ref currentUserQuestionStatus, requestData.IsAnswerCorrect);

            await _trainingRepository.UpdateQuestionStatusAsync(currentUserQuestionStatus);

            return currentUserQuestionStatus;
        }

        private static UserQuestionStatus CreateNewUserQuestionStatus(Guid userId, Guid questionId)
        {
            return new UserQuestionStatus
            {
                QuestionId = questionId,
                UserId = userId,
                IsNew = true,
                PenaltyPoints = 0,
                Rating = Constants.InitialQuestionRating
            };
        }

        private static void UpdateUserQuestionStatusByAnswer(ref UserQuestionStatus userQuestionStatus, bool isAnswerCorrect)
        {
            if (isAnswerCorrect)
            {
                if (userQuestionStatus.IsNew)
                {
                    userQuestionStatus.IsNew = false;
                    userQuestionStatus.PenaltyPoints = 0;
                    userQuestionStatus.Rating = Constants.InitialQuestionRating;
                }

                if (userQuestionStatus.PenaltyPoints > 0)
                {
                    userQuestionStatus.PenaltyPoints--;
                }
                else if (userQuestionStatus.Rating > Constants.MinQuestionRating)
                {
                    userQuestionStatus.Rating--;
                }
            }
            else
            {
                if (userQuestionStatus.IsNew)
                {
                    userQuestionStatus.PenaltyPoints = 0;
                    userQuestionStatus.Rating = Constants.InitialQuestionRating;
                }
                else
                {
                    if (userQuestionStatus.PenaltyPoints < int.MaxValue)
                    {
                        userQuestionStatus.PenaltyPoints++;
                    }
                    userQuestionStatus.Rating = Constants.MaxQuestionRating;
                }
            }
        }

    }
}
