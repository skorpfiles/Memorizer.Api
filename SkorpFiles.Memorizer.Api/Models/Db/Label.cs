using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("sLabel")]
    public class Label:ObjectWithLifetime
    {
        [Key]
        public Guid LabelId { get; set; }
        public string? LabelName { get; set; }
        public Guid OwnerId { get; set; }
    }
}
