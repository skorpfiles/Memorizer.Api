using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;

namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Training
{
    public class TrainingResultRequest
    {
        public Guid QuestionId { get; set; }
        public DateTime TrainingStartTime { get; set; }
        public bool IsAnswerCorrect { get; set; }
        public int AnswerTimeMilliseconds { get; set; }
        public List<GivenTypedAnswer>? GivenTypedAnswers { get; set; }
    }
}
