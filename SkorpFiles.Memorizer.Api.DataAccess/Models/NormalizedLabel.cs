using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("rNormalizedLabel", Schema = Constants.MemorizerSchemaName)]
    public class NormalizedLabel: ObjectWithCreationTime
    {
        [Key]
        public Guid NormalizedLabelId { get; set; }
        public string NormalizedLabelName { get; set; } = null!;

        public List<QuestionLabel> QuestionsForNormalizedLabel { get; set; } = [];
        public List<QuestionnaireLabel> QuestionnairesForNormalizedLabel { get; set; } = [];
    }
}
