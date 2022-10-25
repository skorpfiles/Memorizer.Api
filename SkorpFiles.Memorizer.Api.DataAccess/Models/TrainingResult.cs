using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("jTrainingResult", Schema = Constants.MemorizerSchemaName)]
    public class TrainingResult
    {
        [Key]
        public Guid TrainingResultId { get; set; }
        public DateTime TrainingResultRecordingTime { get; set; }
        public string TrainingResultUserId { get; set; }
        public Guid TrainingResultQuestionId { get; set; }
        public bool TrainingResultQuestionHasBeenNew { get; set; }
        public string? TrainingResultUntypedAnswer { get; set; }
        public bool TrainingResultAnswerIsCorrect { get; set; }
        public int TrainingResultRating { get; set; }
        public int TrainingResultPenaltyPoints { get; set; }
        public int TrainingResultTimeMilliseconds { get; set; }

        public ApplicationUser? TrainingResultUser { get; set; }
        public Question? TrainingResultQuestion { get; set; }

        public List<TrainingResultTypedAnswer>? TypedAnswers { get; set; }

        public TrainingResult(string trainingResultUserId)
        {
            TrainingResultUserId = trainingResultUserId;
        }
    }
}
