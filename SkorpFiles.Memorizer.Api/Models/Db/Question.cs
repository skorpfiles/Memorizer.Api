using SkorpFiles.Memorizer.Api.Enums.Db;
using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("rQuestion")]
    public class Question:ObjectWithLifetime
    {
        [Key]
        public Guid QuestionId { get; set; }
        public QuestionType QuestionType { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionUntypedAnswer { get; set; }
        public bool QuestionIsEnabled { get; set; }
        public string? QuestionReference { get; set; }
        public bool QuestionIsFixed { get; set; }
        public Guid QuestionnaireId { get; set; }
    }
}
