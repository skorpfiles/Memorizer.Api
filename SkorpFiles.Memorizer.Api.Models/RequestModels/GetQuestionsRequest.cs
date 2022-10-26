using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class GetQuestionsRequest
    {
        public Guid? QuestionnaireId { get; set; }
        public int? QuestionnaireCode { get; set; }
        public IEnumerable<string>? LabelsNames { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public QuestionSortField SortField { get; set; } = QuestionSortField.AddedTime;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    }
}
