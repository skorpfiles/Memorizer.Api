namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class PostTrainingRequest
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? LastTime { get; set; }
        public string? LengthType { get; set; }
        public int? TimeMinutes { get; set; }
        public List<Guid>? QuestionnairesIds { get; set; }
    }
}
