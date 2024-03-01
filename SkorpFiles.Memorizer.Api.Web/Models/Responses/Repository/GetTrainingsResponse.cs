using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Responses.Repository
{
    public class GetTrainingsResponse : PaginatedCollectionResponse
    {
        public List<ApiEntities.Training>? Trainings { get; set; }
    }
}
