namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class QuestionToUpdate:Question
    {
        public IEnumerable<Guid>? LabelsIds { get; set; }
        public IEnumerable<string>? TypedAnswers { get; set; }
    }
}
