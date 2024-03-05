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
    public class TrainingRepository:RepositoryBase, ITrainingRepository
    {
        private readonly IMapper _mapper;
        public TrainingRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Api.Models.Question>> GetQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds)
        {
            string userIdString = userId.ToAspNetUserIdString();

            var query = await (from q in DbContext.Questions
                               .Include(q => q.TypedAnswers)
                               .Include(q=>q.Questionnaire)
                               .ThenInclude(q=>q!.Owner)
                               where !q.ObjectIsRemoved && questionnairesIds.Contains(q.QuestionnaireId)
                               join qu in DbContext.QuestionsUsers
                               on q.QuestionId equals qu.QuestionId into quGroup
                               from quo in quGroup.DefaultIfEmpty()
                               where quo == null || quo.UserId == userIdString
                               select new
                               {
                                   Question = q,
                                   QuestionUser = quo
                               }).ToListAsync();

            return query.Select(queryRecord =>
            {
                var resultQuestion = _mapper.Map<Api.Models.Question>(queryRecord.Question);
                if (queryRecord.QuestionUser != null)
                    resultQuestion.MyStatus = _mapper.Map<UserQuestionStatus>(queryRecord.QuestionUser);
                return resultQuestion;
            }).ToList();
        }

        public async Task UpdateQuestionStatusAsync(UserQuestionStatus newQuestionStatus)
        {
            if (newQuestionStatus == null)
                throw new ArgumentNullException(nameof(newQuestionStatus));
            string userIdString = newQuestionStatus.UserId.ToAspNetUserIdString();

            var questionUser = await (from uq in DbContext.QuestionsUsers
                               .Include(uq => uq.Question)
                               .ThenInclude(q => q!.Questionnaire)
                                      where uq.UserId == userIdString &&
                                      uq.QuestionId == newQuestionStatus.QuestionId
                                      select uq).SingleOrDefaultAsync();
            if (questionUser == null)
            {
                await UpdateNonExistingQuestionStatusAsync(newQuestionStatus);
            }
            else
            {
                await UpdateExistingQuestionStatusAsync(questionUser, newQuestionStatus);
            }
        }

        public async Task<UserQuestionStatus?> GetUserQuestionStatusAsync(Guid userId, Guid questionId)
        {
            string userIdString = userId.ToAspNetUserIdString();
            var questionUser = await (from uq in DbContext.QuestionsUsers
                               where uq.UserId == userIdString &&
                               uq.QuestionId == questionId
                               select uq).SingleOrDefaultAsync();
            return questionUser != null ? _mapper.Map<UserQuestionStatus>(questionUser) : null;
        }

        private async Task UpdateNonExistingQuestionStatusAsync(UserQuestionStatus newQuestionStatus)
        {
            var question = await (from q in DbContext.Questions.Include(q => q.Questionnaire)
                                  where q.QuestionId == newQuestionStatus.QuestionId
                                  select q).SingleOrDefaultAsync();
            if (question != null && !question.ObjectIsRemoved &&
                question.Questionnaire != null && !question.Questionnaire.ObjectIsRemoved)
            {
                Utils.CheckQuestionnaireAvailabilityForUser(newQuestionStatus.UserId,
                    Guid.Parse(question.Questionnaire.OwnerId), question.Questionnaire.QuestionnaireAvailability);
                var questionUser = _mapper.Map<QuestionUser>(newQuestionStatus);
                questionUser.ObjectCreationTimeUtc = DateTime.UtcNow;
                await DbContext.QuestionsUsers.AddAsync(questionUser);
                await DbContext.SaveChangesAsync();
            }
            else
            {
                throw new ObjectNotFoundException("Question with such ID is not found.");
            }
        }

        private async Task UpdateExistingQuestionStatusAsync(QuestionUser questionUser, UserQuestionStatus newQuestionStatus)
        {
            if (questionUser.Question != null && !questionUser.Question.ObjectIsRemoved &&
                    questionUser.Question.Questionnaire != null && !questionUser.Question.Questionnaire.ObjectIsRemoved)
            {
                Utils.CheckQuestionnaireAvailabilityForUser(newQuestionStatus.UserId,
                    Guid.Parse(questionUser.Question.Questionnaire.OwnerId), questionUser.Question.Questionnaire.QuestionnaireAvailability);
                questionUser.QuestionUserIsNew = newQuestionStatus.IsNew;
                questionUser.QuestionUserRating = newQuestionStatus.Rating;
                questionUser.QuestionUserPenaltyPoints = newQuestionStatus.PenaltyPoints;
                await DbContext.SaveChangesAsync();
            }
            else
            {
                throw new ObjectNotFoundException("Question with such ID is not found.");
            }
        }
    }
}
