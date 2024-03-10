namespace SkorpFiles.Memorizer.Api.Web.Models.ApiEntities
{
    public class QuestionForTraining:ExistingQuestion
    {
        public QuestionnaireForTraining? Questionnaire {  get; set; }
    }
}
