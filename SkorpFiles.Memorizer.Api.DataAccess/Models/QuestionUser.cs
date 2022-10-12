using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("nnQuestionUser", Schema = Constants.MemorizerSchemaName)]
    public class QuestionUser:ObjectWithCreationTime
    {
        [Key]
        public Guid QuestionUserId { get; set; }
        public string UserId { get; set; }
        public Guid QuestionId { get; set; }
        public bool QuestionUserIsNew { get; set; }
        public int QuestionUserRating { get; set; }
        public int QuestionUserPenaltyPoints { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public Question? Question { get; set; }


        public QuestionUser(string userId)
        {
            UserId = userId;
        }
    }
}
