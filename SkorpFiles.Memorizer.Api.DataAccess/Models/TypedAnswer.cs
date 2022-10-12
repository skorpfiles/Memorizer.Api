using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("rTypedAnswer", Schema = Constants.MemorizerSchemaName)]
    public class TypedAnswer:ObjectWithLifetime
    {
        [Key]
        public Guid TypedAnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public int TypedAnswerQuestionId { get; set; }
        public string TypedAnswerText { get; set; }

        public Question? Question { get; set; }

        public TypedAnswer(string typedAnswerText)
        {
            TypedAnswerText = typedAnswerText;
        }
    }
}
