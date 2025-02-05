using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class TrainingRepository(ApplicationDbContext dbContext, IMapper mapper) : RepositoryBase(dbContext), ITrainingRepository
    {
        public async Task<IEnumerable<Api.Models.ExistingQuestion>> GetQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId must not be empty.");

            ArgumentNullException.ThrowIfNull(questionnairesIds);

            if (!questionnairesIds.Any())
                throw new ArgumentException("QuestionnairesIds must not be empty.");

            string userIdString = userId.ToAspNetUserIdString()!;

            var query = await (from q in DbContext.Questions
                               .Include(q => q.TypedAnswers)
                               .Include(q=>q.Questionnaire)
                               .ThenInclude(q=>q!.Owner)
                               where !q.ObjectIsRemoved && questionnairesIds.Contains(q.QuestionnaireId)
                               join qu in DbContext.QuestionsUsers.Where(qu=>qu.UserId == userIdString)
                               on q.QuestionId equals qu.QuestionId into quGroup
                               from quo in quGroup.DefaultIfEmpty()
                               select new
                               {
                                   Question = q,
                                   QuestionUser = quo
                               }).ToListAsync();

            return query.Select(queryRecord =>
            {
                var resultQuestion = mapper.Map<Api.Models.ExistingQuestion>(queryRecord.Question);
                if (queryRecord.QuestionUser != null)
                    resultQuestion.MyStatus = mapper.Map<UserQuestionStatus>(queryRecord.QuestionUser);
                return resultQuestion;
            }).ToList();
        }

        public async Task UpdateQuestionStatusAsync(UserQuestionStatus newQuestionStatus, Api.Models.TrainingResult trainingResult, Api.Models.QuestionStatus defaultQuestionStatus)
        {
            ArgumentNullException.ThrowIfNull(newQuestionStatus);

            if (newQuestionStatus.UserId == Guid.Empty)
                throw new ArgumentException("UserId must not be empty.");

            string userIdString = newQuestionStatus.UserId.ToAspNetUserIdString()!;

            var questionUser = await (from uq in DbContext.QuestionsUsers
                               .Include(uq => uq.Question)
                               .ThenInclude(q => q!.Questionnaire)
                                      where uq.UserId == userIdString &&
                                      uq.QuestionId == newQuestionStatus.QuestionId
                                      select uq).SingleOrDefaultAsync();

            if (questionUser == null)
            {
                await UpdateNonExistingQuestionStatusAndSaveChangesAsync(newQuestionStatus, trainingResult, defaultQuestionStatus);
            }
            else
            {
                await UpdateExistingQuestionStatusAsync(questionUser, newQuestionStatus, trainingResult);
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<UserQuestionStatus?> GetUserQuestionStatusAsync(Guid userId, Guid questionId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId must not be empty.");

            if (questionId == Guid.Empty)
                throw new ArgumentException("QuestionId must not be empty.");

            string userIdString = userId.ToAspNetUserIdString()!;
            var questionUser = await (from uq in DbContext.QuestionsUsers
                               where uq.UserId == userIdString &&
                               uq.QuestionId == questionId
                               select uq).SingleOrDefaultAsync();
            return questionUser != null ? mapper.Map<UserQuestionStatus>(questionUser) : null;
        }

        private async Task<bool> UpdateNonExistingQuestionStatusAndSaveChangesAsync(UserQuestionStatus newQuestionStatus, Api.Models.TrainingResult trainingResult, Api.Models.QuestionStatus defaultQuestionStatus)
        {
            var question = await (from q in DbContext.Questions.Include(q => q.Questionnaire)
                                  where q.QuestionId == newQuestionStatus.QuestionId
                                  select q).SingleOrDefaultAsync();
            if (question != null && !question.ObjectIsRemoved &&
                question.Questionnaire != null && !question.Questionnaire.ObjectIsRemoved)
            {
                Utils.CheckQuestionnaireAvailabilityForUser(newQuestionStatus.UserId,
                    Guid.Parse(question.Questionnaire.OwnerId), question.Questionnaire.QuestionnaireAvailability);
                var questionUser = mapper.Map<QuestionUser>(newQuestionStatus);
                questionUser.ObjectCreationTimeUtc = DateTime.UtcNow;

                await DbContext.QuestionsUsers.AddAsync(questionUser);

                trainingResult.InitialQuestionStatus = defaultQuestionStatus;
                await LogTrainingResultAsync(trainingResult);

                return await Utils.ExecuteInDangerOfMultipleAddUniqueRecordsAsync(async () => await DbContext.SaveChangesAsync());
            }
            else
            {
                throw new ObjectNotFoundException("Question with such ID is not found.");
            }
        }

        private async Task UpdateExistingQuestionStatusAsync(QuestionUser questionUser, UserQuestionStatus newQuestionStatus, Api.Models.TrainingResult trainingResult)
        {
            if (questionUser.Question != null && !questionUser.Question.ObjectIsRemoved &&
                    questionUser.Question.Questionnaire != null && !questionUser.Question.Questionnaire.ObjectIsRemoved)
            {
                Utils.CheckQuestionnaireAvailabilityForUser(newQuestionStatus.UserId,
                    Guid.Parse(questionUser.Question.Questionnaire.OwnerId), questionUser.Question.Questionnaire.QuestionnaireAvailability);

                trainingResult.InitialQuestionStatus = new QuestionStatus
                {
                    IsNew = questionUser.QuestionUserIsNew,
                    Rating = questionUser.QuestionUserRating,
                    PenaltyPoints = questionUser.QuestionUserPenaltyPoints
                };

                questionUser.QuestionUserIsNew = newQuestionStatus.IsNew;
                questionUser.QuestionUserRating = newQuestionStatus.Rating;
                questionUser.QuestionUserPenaltyPoints = newQuestionStatus.PenaltyPoints;

                await LogTrainingResultAsync(trainingResult);
            }
            else
            {
                throw new ObjectNotFoundException("Question with such ID is not found.");
            }
        }

        private async Task LogTrainingResultAsync(Api.Models.TrainingResult trainingResult)
        {
            ArgumentNullException.ThrowIfNull(trainingResult);

            if (trainingResult.QuestionId == Guid.Empty)
                throw new ArgumentException($"QuestionId must not be null.");
            if (trainingResult.UserId == Guid.Empty)
                throw new ArgumentException($"UserId must not be null.");

            await DbContext.TrainingResults.AddAsync(mapper.Map<Models.TrainingResult>(trainingResult));
        }
    }
}
