using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("nnQuestionUser", Schema = Constants.MemorizerSchemaName)]
    [Index(nameof(UserId), nameof(QuestionId), IsUnique = true)] //to forbid adding more than one unique combinations of user ID and question ID.
    public class QuestionUser:ObjectWithCreationTime
    {
        [Key]
        public Guid QuestionUserId { get; set; }
        public string UserId { get; set; } = null!;
        public Guid QuestionId { get; set; }
        public bool QuestionUserIsNew { get; set; }
        public int QuestionUserRating { get; set; }
        public int QuestionUserPenaltyPoints { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
        public Question? Question { get; set; }
    }
}
