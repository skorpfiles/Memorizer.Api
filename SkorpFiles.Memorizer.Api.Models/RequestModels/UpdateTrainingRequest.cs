using SkorpFiles.Memorizer.Api.Models.Enums;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class UpdateTrainingRequest
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public bool RefreshLastTime { get; set; }
        public TrainingLengthType? LengthType { get; set; }
        public int? QuestionsCount { get; set; }
        public int? TimeMinutes { get; set; }
        public decimal NewQuestionsFraction { get; set; }
        public decimal PenaltyQuestionsFraction { get; set; }
        public IEnumerable<Guid>? QuestionnairesIds { get; set; }
    }
}
