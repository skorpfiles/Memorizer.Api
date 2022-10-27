namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class UserQuestionStatus
    {
        public bool IsNew { get; set; }
        public int Rating { get; set; }
        public int PenaltyPoints { get; set; }
        public double? AverageTrainingTimeSeconds { get; set; }
    }
}
