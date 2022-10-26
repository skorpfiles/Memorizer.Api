using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class GetQuestionsRequest:CollectionRequest
    {
        public Guid? QuestionnaireId { get; set; }
        public int? QuestionnaireCode { get; set; }
        public IEnumerable<string>? LabelsNames { get; set; }
    }
}
