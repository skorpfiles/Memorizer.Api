using SkorpFiles.Memorizer.Api.Models.Abstract;
using SkorpFiles.Memorizer.Api.Models.Enums;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class GetQuestionsForTrainingResult:Entity
    {
        public QuestionType QuestionType { get; set; }
        public string QuestionText { get; set; } = null!;
        public string? QuestionUntypedAnswer { get; set; }
        public bool QuestionIsEnabled { get; set; }
        public string? QuestionReference { get; set; }
        public int QuestionEstimatedTrainingTimeSeconds { get; set; }
        public string? TypedAnswersJson { get; set; }
        public Guid? QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; } = null!;
        public bool? QuestionUserIsNew { get; set; }
        public int? QuestionUserRating { get; set; }
        public int? QuestionUserPenaltyPoints { get; set; }
        public int QuestionActualTrainingTimeSeconds { get; set; }
    }
}
