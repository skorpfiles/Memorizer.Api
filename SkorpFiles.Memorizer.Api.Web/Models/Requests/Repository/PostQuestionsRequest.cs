using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;

namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class PostQuestionsRequest
    {
        public Guid? QuestionnaireId { get; set; }
        public int? QuestionnaireCode { get; set; }
        public IEnumerable<QuestionToUpdate>? CreatedQuestions { get; set; }
        public IEnumerable<QuestionToUpdate>? UpdatedQuestions { get; set; }
        public IEnumerable<QuestionIdentifier>? DeletedQuestions { get; set; }
    }
}
