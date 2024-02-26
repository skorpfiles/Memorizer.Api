using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Responses.Repository
{
    public class GetLabelsResponse : PaginatedCollectionResponse
    {
        public List<Label>? Labels { get; set; }
    }
}
