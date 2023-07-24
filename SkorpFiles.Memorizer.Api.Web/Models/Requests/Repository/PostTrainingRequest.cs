namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class PostTrainingRequest
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public bool? RefreshLastTime { get; set; }
        public string? LengthType { get; set; }
        public int? QuestionsCount { get; set; }
        public int? TimeMinutes { get; set; }
        public List<Guid>? QuestionnairesIds { get; set; }
    }
}
