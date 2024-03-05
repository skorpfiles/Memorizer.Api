using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;

namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class PostMyStatusRequest
    {
        public IEnumerable<UserQuestionStatusForUpdate>? Items { get; set; }
    }
}
