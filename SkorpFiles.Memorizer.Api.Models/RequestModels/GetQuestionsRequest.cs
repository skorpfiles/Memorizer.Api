using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class GetQuestionsRequest:GetCollectionRequest
    {
        public Guid? QuestionnaireId { get; set; }
        public int? QuestionnaireCode { get; set; }
        public IEnumerable<string>? LabelsNames { get; set; }
        public QuestionSortField SortField { get; set; } = QuestionSortField.AddedTime;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    }
}
