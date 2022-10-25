using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Exceptions;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class EditingRepository : RepositoryBase, IEditingRepository
    {
        private readonly IMapper _mapper;
        public EditingRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext) 
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Questionnaire>> GetQuestionnairesAsync(Guid userId,
            GetQuestionnairesRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var userIdString = userId.ToAspNetUserIdString();
            var ownerIdString = request.OwnerId?.ToAspNetUserIdString();

            IQueryable<Models.Questionnaire> foundQuestionnaires = DbContext.Questionnaires
                .Include(q => q.LabelsForQuestionnaire!)
                .ThenInclude(el => el.Label);

            if (request.LabelsNames != null && request.LabelsNames.Any())
            {
                var labelsIds =
                    from label in DbContext.Labels
                    where request.LabelsNames.Contains(label.LabelName)
                    select label.LabelId;

                var entityLabels =
                    from entityLabel in DbContext.EntitiesLabels
                    where labelsIds.Contains(entityLabel.LabelId)
                    select entityLabel;

                var questionnaireIds =
                    from entityLabel in entityLabels
                    group entityLabel by entityLabel.QuestionnaireId into grouped
                    where grouped.Count() >= request.LabelsNames.Count()
                    select grouped.Key;

                foundQuestionnaires =
                    from questionnaire in foundQuestionnaires
                    where
                        questionnaireIds.Contains(questionnaire.QuestionnaireId)
                    select questionnaire;
            }

            foundQuestionnaires =
                from questionnaire in foundQuestionnaires
                where
                    (request.Origin == null ||
                    (request.Origin == QuestionnaireOrigin.Own && questionnaire.OwnerId == userIdString) ||
                    (request.Origin == QuestionnaireOrigin.Foreign && questionnaire.OwnerId != userIdString)) &&
                    (ownerIdString == null || request.OwnerId!.Value == default || questionnaire.OwnerId == ownerIdString) &&
                    (request.Availability == null || request.Availability == questionnaire.QuestionnaireAvailability) &&
                    (request.PartOfName == null || questionnaire.QuestionnaireName.ToLower().Contains(request.PartOfName))
                select questionnaire;

            switch (request.SortField)
            {
                case QuestionnaireSortField.Name:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.QuestionnaireName); break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.QuestionnaireName); break;
                    }
                    break;
                case QuestionnaireSortField.OwnerName:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.Owner.UserName); break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.Owner.UserName); break;
                    }
                    break;
            }

            foundQuestionnaires = foundQuestionnaires.Page(request.PageNumber, request.PageSize);

            var foundQuestionnairesResult = await foundQuestionnaires.ToListAsync();
            foreach (var questionnaire in foundQuestionnairesResult)
                if (questionnaire?.LabelsForQuestionnaire != null)
                    questionnaire.LabelsForQuestionnaire = questionnaire.LabelsForQuestionnaire.OrderBy(l => l.LabelNumber).ToList();

            return _mapper.Map<IEnumerable<Questionnaire>>(foundQuestionnairesResult);
        }

        public async Task<Questionnaire> GetQuestionnaireAsync(Guid userId, Guid questionnaireId) =>
            await GetQuestionnaireAsync(userId, questionnaireId, null);

        public async Task<Questionnaire> GetQuestionnaireAsync(Guid userId, int questionnaireCode) =>
            await GetQuestionnaireAsync(userId, null, questionnaireCode);

        private async Task<Questionnaire> GetQuestionnaireAsync(Guid userId, Guid? questionnaireId = null, int? questionnaireCode = null)
        {
            if (questionnaireId == null && questionnaireCode == null)
                throw new ArgumentException($"Either {questionnaireId} or {questionnaireCode} should not be null.");

            var questionnaireResult =
                await (from questionnaire in DbContext.Questionnaires
                       .Include(q=>q.LabelsForQuestionnaire!)
                       .ThenInclude(el=>el.Label)
                 where
                     (questionnaireId == null || questionnaire.QuestionnaireId == questionnaireId) &&
                     (questionnaireCode == null || questionnaire.QuestionnaireCode == questionnaireCode)
                 select questionnaire).SingleOrDefaultAsync();

            if (questionnaireResult != null && questionnaireResult.QuestionnaireAvailability == QuestionnaireAvailability.Private && Guid.Parse(questionnaireResult.OwnerId) != userId)
                throw new AccessDeniedForUserException("Unable to get details about a private questionnaire to a foreign user.");

            return _mapper.Map<Questionnaire>(questionnaireResult);
        }
    }
}
