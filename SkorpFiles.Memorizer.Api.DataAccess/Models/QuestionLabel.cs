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
        public Guid LabelId { get; set; }

        public Question? Question { get; set; }
        public Label? Label { get; set; }
    }
}
