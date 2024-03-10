namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class QuestionnaireForTraining
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string? Name { get; set; }
        public Guid OwnerId { get; set; }
        public string? OwnerName { get; set; }
    }
}
