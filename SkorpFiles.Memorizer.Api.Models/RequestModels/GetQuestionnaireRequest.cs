using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class GetQuestionnaireRequest
    {
        public QuestionnaireOrigin? Origin { get; set; }
        public Guid? OwnerId { get; set; }
        public QuestionnaireAvailability? Availability { get; set; }
        public string? PartOfName { get; set; }
        public QuestionnaireSortField SortField { get; set; } = QuestionnaireSortField.Name;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public IEnumerable<string>? LabelsNames { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
