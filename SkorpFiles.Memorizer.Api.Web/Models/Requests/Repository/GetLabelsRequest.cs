using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class GetLabelsRequest : CollectionRequest
    {
        public string? Origin { get; set; }
        public Guid OwnerId { get; set; }
        public string? PartOfName { get; set; }
    }
}
