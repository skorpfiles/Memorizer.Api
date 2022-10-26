using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class GetQuestionnairesRequest:CollectionRequest
    {
        public string? Origin { get; set; }
        public Guid OwnerId { get; set; }
        public string? Availability { get; set; }
        public string? PartOfName { get; set; }
        public List<string>? LabelsNames { get; set; }
    }
}
