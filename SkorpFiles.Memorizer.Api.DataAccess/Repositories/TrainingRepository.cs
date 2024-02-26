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

        public async Task<IEnumerable<Api.Models.Question>> GetQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds)
        {
            string userIdString = userId.ToAspNetUserIdString();

            var query = await (from q in DbContext.Questions
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
