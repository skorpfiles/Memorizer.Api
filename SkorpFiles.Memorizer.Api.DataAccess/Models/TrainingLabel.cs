using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("nnTrainingLabel", Schema = Constants.MemorizerSchemaName)]
    public class TrainingLabel:ObjectWithCreationTime
    {
        [Key]
        public Guid TrainingLabelId { get; set; }
        public Guid TrainingId { get; set; }
        public Guid NormalizedLabelId { get; set; }
        public string TrainingLabelName { get; set; } = null!;

        [ForeignKey(nameof(TrainingId))]
        public Training? Training { get; set; }

        [ForeignKey(nameof(NormalizedLabelId))]
        public NormalizedLabel? NormalizedLabel { get; set; }
    }
}
