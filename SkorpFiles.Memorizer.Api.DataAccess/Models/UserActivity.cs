using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
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

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? ApplicationUser { get; set; }

        public UserActivity(string userName, string userId)
        {
            UserName = userName;
            UserId = userId;
        }
    }
}
