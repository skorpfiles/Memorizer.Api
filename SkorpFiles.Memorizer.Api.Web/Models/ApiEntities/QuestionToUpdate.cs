namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class QuestionToUpdate:Question
    {
        public IEnumerable<string>? TypedAnswers { get; set; }
    }
}
