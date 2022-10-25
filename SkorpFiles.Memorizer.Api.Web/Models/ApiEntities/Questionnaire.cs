namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class Questionnaire
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string? Name { get; set; }
        public string? Availability { get; set; }
        public Guid OwnerId { get; set; }
        public int QuestionsCount { get; set; }
        public List<Label>? Labels { get; set; }
    }
}
