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
        public string LabelName { get; set; }
        public string OwnerId { get; set; }

        public List<EntityLabel>? EntitiesForLabel { get; set; }

        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }

        public Label(string labelName, string ownerId)
        {
            LabelName = labelName;
            OwnerId = ownerId;
        }
    }
}
