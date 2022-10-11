using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("rUserActivity")]
    public class UserActivity:ObjectWithLifetime
    {
        [Key]
        public Guid UserActivityId { get; set; }
        public Guid UserId { get; set; }
        public int UserCode { get; set; }
        public string? UserName { get; set; }
        public bool UserIsEnabled { get; set; }
    }
}
