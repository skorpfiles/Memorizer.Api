using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("nnQuestionnaireLabel", Schema = Constants.MemorizerSchemaName)]
    public class QuestionnaireLabel: ObjectWithCreationTime
    {
        [Key]
        public Guid QuestionnaireLabelId { get; set; }

        public Guid QuestionnaireId { get; set; }

        public Guid NormalizedLabelId { get; set; }

        public string QuestionnaireLabelName { get; set; } = null!;

        public bool QuestionnaireLabelIsAlive { get; set; }

        [ForeignKey(nameof(QuestionnaireId))]
        public Questionnaire? Questionnaire { get; set; }

        [ForeignKey(nameof(NormalizedLabelId))]
        public NormalizedLabel? NormalizedLabel { get; set; }
    }
}
