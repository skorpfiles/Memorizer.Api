using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;

namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class PutQuestionnaireRequest
    {
        public string? Name { get; set; }
        public string? Availability { get; set; }
        public List<Label>? Labels { get; set; }
    }
}
