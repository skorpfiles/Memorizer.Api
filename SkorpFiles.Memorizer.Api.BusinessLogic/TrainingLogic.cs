using AutoMapper;
using SkorpFiles.Memorizer.Api.BusinessLogic.Training;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Models.Utils;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public class TrainingLogic(ITrainingRepository trainingRepository, IMapper mapper) : ITrainingLogic
    {
        public async Task<IEnumerable<Api.Models.ExistingQuestion>> SelectQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingOptions options)
        {
            if (options.NewQuestionsFraction < 0 || options.PrioritizedPenaltyQuestionsFraction < 0)
                throw new IncorrectTrainingOptionsException(Constants.NegativeFractionsMessage);

            if (options.NewQuestionsFraction + options.PrioritizedPenaltyQuestionsFraction > 1)
                throw new IncorrectTrainingOptionsException(Constants.SumOfFractionsCannotBeMoreThan1Message);

            if (options.LengthValue <= 0)
                throw new IncorrectTrainingOptionsException(Constants.NonPositiveLengthValueMessage);

            var allQuestions = (await trainingRepository.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ToList();
            var questionsListsCollection = new TrainingBuilder(allQuestions);
            var questionsList = questionsListsCollection.MakeQuestionsListForTraining(options, mapper);

            foreach(var question in questionsList)
            {
                question.MyStatus ??= new UserQuestionStatus
                {
                    IsNew = true,
                    PenaltyPoints = 0,
                    Rating = Restrictions.InitialQuestionRating,
                    AverageTrainingTimeSeconds = question.EstimatedTrainingTimeSeconds
                };
            }

            return questionsList;
        }

        public async Task<UserQuestionStatus> UpdateQuestionStatusAsync(Guid userId, TrainingResult trainingResult)
        {
            ArgumentNullException.ThrowIfNull(trainingResult);

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            if (trainingResult.QuestionId == Guid.Empty)
                throw new ArgumentException("QuestionId cannot be empty.");

            if (trainingResult.AnswerTimeMilliseconds <= 0)
                throw new ArgumentException("AnswerTime must be positive.");

            var currentUserQuestionStatus = await trainingRepository.GetUserQuestionStatusAsync(userId, trainingResult.QuestionId);
            currentUserQuestionStatus ??= CreateNewUserQuestionStatus(userId, trainingResult.QuestionId);

            UpdateUserQuestionStatusByAnswer(ref currentUserQuestionStatus, trainingResult.IsAnswerCorrect);

            trainingResult.ResultQuestionStatus = new QuestionStatus { IsNew = currentUserQuestionStatus.IsNew, Rating = currentUserQuestionStatus.Rating, PenaltyPoints = currentUserQuestionStatus.PenaltyPoints };
            trainingResult.RecordingTime = DateTime.UtcNow;
            trainingResult.UserId = userId;

            await trainingRepository.UpdateQuestionStatusAsync(currentUserQuestionStatus, trainingResult, CreateNewQuestionStatus());

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
                Rating = Restrictions.InitialQuestionRating
            };
        }

        private static QuestionStatus CreateNewQuestionStatus() => new() { IsNew = true, PenaltyPoints = 0, Rating = Restrictions.InitialQuestionRating };

        private static void UpdateUserQuestionStatusByAnswer(ref UserQuestionStatus userQuestionStatus, bool isAnswerCorrect)
        {
            if (isAnswerCorrect)
            {
                if (userQuestionStatus.IsNew)
                {
                    userQuestionStatus.IsNew = false;
                    userQuestionStatus.PenaltyPoints = 0;
                    userQuestionStatus.Rating = Restrictions.InitialQuestionRating;
                }

                if (userQuestionStatus.PenaltyPoints > 0)
                {
                    userQuestionStatus.PenaltyPoints--;
                }
                else if (userQuestionStatus.Rating > Restrictions.MinQuestionRating)
                {
                    userQuestionStatus.Rating--;
                }
            }
            else
            {
                if (userQuestionStatus.IsNew)
                {
                    userQuestionStatus.PenaltyPoints = 0;
                    userQuestionStatus.Rating = Restrictions.InitialQuestionRating;
                }
                else
                {
                    if (userQuestionStatus.PenaltyPoints < int.MaxValue)
                    {
                        userQuestionStatus.PenaltyPoints++;
                    }
                    userQuestionStatus.Rating = Restrictions.MaxQuestionRating;
                }
            }
        }

    }
}
