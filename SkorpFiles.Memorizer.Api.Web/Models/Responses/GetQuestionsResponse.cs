using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Responses
{
    public class GetQuestionsResponse:PaginatedCollectionResponse
    {
        public List<ExistingQuestion>? Questions { get; set; }
    }
}
