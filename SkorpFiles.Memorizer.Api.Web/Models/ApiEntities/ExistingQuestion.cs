namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class ExistingQuestion:Question
    {
        public IEnumerable<Label>? Labels { get; set; }
        public IEnumerable<TypedAnswer>? TypedAnswers { get; set; }
    }
}
