namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class PostQuestionnaireRequest
    {
        public Guid? Id { get; set; }
        public int? Code { get; set; }
        public string? Name { get; set; }
        public string? Availability { get; set; }
        public List<Guid>? LabelsIds { get; set; }
    }
}
