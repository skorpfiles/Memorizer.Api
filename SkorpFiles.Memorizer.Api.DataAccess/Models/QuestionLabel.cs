using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("nnQuestionLabel", Schema = Constants.MemorizerSchemaName)]
    public class QuestionLabel:ObjectWithCreationTime
    {
        [Key]
        public Guid QuestionLabelId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid NormalizedLabelId { get; set; }
        public string QuestionLabelName { get; set; } = null!;

        [ForeignKey(nameof(QuestionId))]
        public Question? Question { get; set; }

        [ForeignKey(nameof(NormalizedLabelId))]
        public NormalizedLabel? NormalizedLabel { get; set; }
    }
}
