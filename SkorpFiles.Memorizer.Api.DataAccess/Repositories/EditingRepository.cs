using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

            var foundQuestionnaires =
                (from questionnaire in DbContext.Questionnaires
                join entityLabel in DbContext.EntitiesLabels on questionnaire equals entityLabel.Questionnaire into entityLabelGroup
                from entityLabelObj in entityLabelGroup.DefaultIfEmpty()
                join label in DbContext.Labels on entityLabelObj.Label equals label into labelGroup
                from labelObj in labelGroup.DefaultIfEmpty()
                where
                    (request.Origin == null ||
                    (request.Origin == QuestionnaireOrigin.Own && questionnaire.OwnerId == userIdString) ||
                    (request.Origin == QuestionnaireOrigin.Foreign && questionnaire.OwnerId != userIdString)) &&
                    (ownerIdString == null || request.OwnerId.Value == default || questionnaire.OwnerId == ownerIdString) &&
                    (request.Availability == null || request.Availability == questionnaire.QuestionnaireAvailability) &&
                    (request.PartOfName == null || questionnaire.QuestionnaireName.ToLower().Contains(request.PartOfName)) &&
                    (request.LabelsNames == null || !request.LabelsNames.Any() || request.LabelsNames.Contains(labelObj.LabelName))
                select questionnaire).Distinct();

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

            var result = foundQuestionnaires.ToList();

            if (request.LabelsNames != null && request.LabelsNames.Any())
            {
                Dictionary<string, IEnumerable<Questionnaire>> QuestionnairesForLabels = new Dictionary<string, IEnumerable<Questionnaire>>();
                foreach (var label in request.LabelsNames)
                {

                }
            }
            //return await foundQuestionnaires
            //    .Select(q => new Questionnaire
            //{
            //    Availability = q.Key.QuestionnaireAvailability,
            //    Code = q.Key.QuestionnaireCode,
            //    CreationTimeUtc = q.Key.ObjectCreationTimeUtc,
            //    Id = q.Key.QuestionnaireId,
            //    IsRemoved = q.Key.ObjectIsRemoved,
            //    Name = q.Key.QuestionnaireName,
            //    OwnerId = Guid.Parse(q.Key.OwnerId),
            //    RemovalTimeUtc = q.Key.ObjectRemovalTimeUtc,
            //    //Labels = q.Select(o =>
            //    //    new Label
            //    //    {
            //    //        Id = o.labelObj.LabelId,
            //    //        Name = o.labelObj.LabelName,
            //    //        OwnerId = o.labelObj.OwnerId!=null ? Guid.Parse(o.labelObj.OwnerId) : null,
            //    //        StatusInQuestionnaire = new LabelInQuestionnaire
            //    //        {
            //    //            Number = o.entityLabelObj.LabelNumber,
            //    //            ParentLabelId = o.entityLabelObj.ParentLabelId
            //    //        },
            //    //        CreationTimeUtc = o.labelObj.ObjectCreationTimeUtc,
            //    //        IsRemoved = o.labelObj.ObjectIsRemoved,
            //    //        RemovalTimeUtc = o.labelObj.ObjectRemovalTimeUtc
            //    //    }).ToList()
            //}).ToListAsync();

            return _mapper.Map<IEnumerable<Questionnaire>>(result);
        }
    }
}
