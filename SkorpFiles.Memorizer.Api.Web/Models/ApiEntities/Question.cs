using SkorpFiles.Memorizer.Api.Models.Enums;

namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class Question
    {
        public Guid Id { get; set; }
        public int CodeInQuestionnaire { get; set; }
        public string? Type { get; set; }
        public string? Text { get; set; }
        public string? UntypedAnswer { get; set; }
        public List<TypedAnswer>? TypedAnswers { get; set; }
        public List<Label>? Labels { get; set; }
        public bool Enabled { get; set; }
        public string? Reference { get; set; }
        public bool IsFixed { get; set; }
        public UserQuestionStatus? MyStatus { get; set; }
    }
}
