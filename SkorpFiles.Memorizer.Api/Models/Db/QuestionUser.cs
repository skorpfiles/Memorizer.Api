using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("nnQuestionUser")]
    public class QuestionUser:ObjectWithCreationTime
    {
        public Guid QuestionUserId { get; set; }
        public Guid UserId { get; set; }
        public bool QuestionUserIsNew { get; set; }
        public int QuestionUserRating { get; set; }
        public int QuestionUserPenaltyPoints { get; set; }
    }
}
