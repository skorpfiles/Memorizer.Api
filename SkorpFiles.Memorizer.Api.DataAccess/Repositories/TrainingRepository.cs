using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
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

        public async Task<IEnumerable<Api.Models.Question>> GetQuestionsForTrainingAsync(Guid userId, Guid trainingId)
        {
            string userIdString = userId.ToAspNetUserIdString();

            var query = await (from t in DbContext.Trainings
                               where !t.ObjectIsRemoved &&
                               t.TrainingId == trainingId &&
                               t.OwnerId == userIdString
                               join tq in DbContext.TrainingsQuestionnaires
                               .Include(tq => tq.Questionnaire)
                               on t.TrainingId equals tq.TrainingId
                               where !tq.Questionnaire.ObjectIsRemoved
                               join q in DbContext.Questions
                               on tq.Questionnaire.QuestionnaireId equals q.QuestionnaireId
                               where !q.ObjectIsRemoved
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
                    resultQuestion.MyStatus = _mapper.Map<Api.Models.UserQuestionStatus>(queryRecord.QuestionUser);
                return resultQuestion;
            }).ToList();
        }

        public Task UpdateQuestionStatusAsync(Api.Models.UserQuestionStatus questionStatus)
        {
            throw new NotImplementedException();
        }
    }
}
