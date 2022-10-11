using SkorpFiles.Memorizer.Api.Enums.Db;
using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("rQuestionnaire")]
    public class Questionnaire:ObjectWithLifetime
    {
        public Guid QuestionnaireId { get; set; }
        public int QuestionnaireCode { get; set; }
        public string? QuestionnaireName { get; set; }
        public QuestionnaireAvailability QuestionnaireAvailability { get; set; }
        public Guid OwnerId { get; set; }
    }
}
