using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
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
        public EditingRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Questionnaire>> GetQuestionnairesAsync(Guid userId,
            GetQuestionnaireRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var userIdString = userId.ToAspNetUserIdString();
            var ownerIdString = request.OwnerId?.ToAspNetUserIdString();

            var foundQuestionnaires =
                from questionnaire in DbContext.Questionnaires
                join entityLabel in DbContext.EntitiesLabels on questionnaire equals entityLabel.Questionnaire
                join label in DbContext.Labels on entityLabel.Label equals label
                where
                    ((request.Origin == QuestionnaireOrigin.Own && questionnaire.OwnerId == userIdString) ||
                    (request.Origin == QuestionnaireOrigin.Foreign && questionnaire.OwnerId != userIdString)) &&
                    (ownerIdString == null || questionnaire.OwnerId == ownerIdString) &&
                    (request.Availability == null || request.Availability == questionnaire.QuestionnaireAvailability) &&
                    (request.PartOfName == null || questionnaire.QuestionnaireName.ToLowerInvariant().Contains(request.PartOfName))
                select questionnaire;

            switch(request.SortField)
            {
                case QuestionnaireSortField.Name:
                    switch(request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.QuestionnaireName);break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.QuestionnaireName);break;
                    }
                    break;
                case QuestionnaireSortField.OwnerName:
                    switch(request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.Owner.UserName); break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.Owner.UserName); break;
                    }
                    break;
            }

            foundQuestionnaires = foundQuestionnaires.Page(request.PageNumber, request.PageSize);

            return foundQuestionnaires.Select(q => new Questionnaire
            {
                Availability = q.QuestionnaireAvailability,
                Code = q.QuestionnaireCode,
                CreatedTimeUtc = q.ObjectCreationTimeUtc,
                Id = q.QuestionnaireId,
                IsRemoved = q.ObjectIsRemoved,
                Name = q.QuestionnaireName,
                OwnerId = Guid.Parse(q.OwnerId),
                RemovalTimeUtc = q.ObjectRemovalTimeUtc
            });
        }
    }
}
