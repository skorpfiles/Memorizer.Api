using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("rUserActivity", Schema = Constants.MemorizerSchemaName)]
    public class UserActivity:ObjectWithLifetime
    {
        [Key]
        public Guid UserActivityId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserCode { get; set; }
        public string UserName { get; set; }
        public bool UserIsEnabled { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        public UserActivity(string userName, string userId)
        {
            UserName = userName;
            UserId = userId;
        }
    }
}
