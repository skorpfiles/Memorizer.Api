namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class ExistingQuestion:Question
    {
        public IEnumerable<TypedAnswer>? TypedAnswers { get; set; }
    }
}
