using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;

namespace SkorpFiles.Memorizer.Api.Web.Models.Responses.Training
{
    public class StartTrainingResponse
    {
        public string? Name {  get; set; }
        public List<QuestionForTraining>? Questions { get; set; }
    }
}
