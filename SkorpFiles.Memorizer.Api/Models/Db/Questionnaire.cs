using SkorpFiles.Memorizer.Api.Enums.Db;
using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("rQuestionnaire", Schema = Constants.MemorizerSchemaName)]
    public class Questionnaire:ObjectWithLifetime
    {
        [Key]
        public Guid QuestionnaireId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionnaireCode { get; set; }
        public string QuestionnaireName { get; set; }
        public QuestionnaireAvailability QuestionnaireAvailability { get; set; }
        public string OwnerId { get; set; }

        public List<EntityLabel>? LabelsForQuestionnaire { get; set; }
        public List<Question>? Questions { get; set; }

        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }

        public Questionnaire(string questionnaireName, string ownerId)
        {
            QuestionnaireName = questionnaireName;
            OwnerId = ownerId;
        }
    }
}