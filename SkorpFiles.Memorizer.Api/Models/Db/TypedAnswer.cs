using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("rTypedAnswer")]
    public class TypedAnswer:ObjectWithLifetime
    {
        public Guid TypedAnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public int TypedAnswerQuestionId { get; set; }
        public string? TypedAnswerText { get; set; }
        public Guid OwnerId { get; set; }
    }
}
