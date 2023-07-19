using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("sLabel", Schema = Constants.MemorizerSchemaName)]
    public class Label:ObjectWithLifetime
    {
        [Key]
        public Guid LabelId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabelCode { get; set; }
        public string LabelName { get; set; } = null!;
        public string OwnerId { get; set; } = null!;

        public List<EntityLabel>? EntitiesForLabel { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public ApplicationUser? Owner { get; set; }
    }
}
