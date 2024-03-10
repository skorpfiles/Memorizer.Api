using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("rTraining", Schema = Constants.MemorizerSchemaName)]
    public class Training : ObjectWithLifetime
    {
        [Key]
        public Guid TrainingId { get; set; }
        public string OwnerId { get; set; } = null!;
        public string TrainingName { get; set; } = null!;

        [Column("TrainingLastTime")]
        public DateTime TrainingLastTimeUtc { get; set; }
        public TrainingLengthType TrainingLengthType { get; set; }
        public int TrainingQuestionsCount { get; set; }
        public int TrainingTimeMinutes { get; set; }

        [Column(TypeName = "decimal(4,3)")]
        public decimal TrainingNewQuestionsFraction {  get; set; }

        [Column(TypeName = "decimal(4,3)")]
        public decimal TrainingPenaltyQuestionsFraction {  get; set; }

        [ForeignKey(nameof(OwnerId))]
        public ApplicationUser? Owner { get; set; }

        public List<TrainingQuestionnaire>? QuestionnairesForTraining { get; set; }

    }
}
