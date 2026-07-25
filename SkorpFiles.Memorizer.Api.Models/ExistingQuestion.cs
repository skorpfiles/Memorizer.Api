namespace SkorpFiles.Memorizer.Api.Models
{
    public class ExistingQuestion:Question
    {
        public IEnumerable<string>? Labels { get; set; }
        public IEnumerable<TypedAnswer>? TypedAnswers { get; set; }
    }
}
