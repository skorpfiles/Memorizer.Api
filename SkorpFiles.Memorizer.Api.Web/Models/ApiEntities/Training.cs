namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class Training
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime LastTime { get; set; }
        public string? LengthType { get; set; }
        public int QuestionsCount { get; set; }
        public int TimeMinutes { get;set; }
        public decimal? NewQuestionsFraction {  get; set; }
        public decimal? PenaltyQuestionsFraction {  get; set; }
        public List<Questionnaire>? Questionnaires { get; set; }
    }
}
