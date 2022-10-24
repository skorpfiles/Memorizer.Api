using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("jTrainingResultTypedAnswer", Schema = Constants.MemorizerSchemaName)]
    public class TrainingResultTypedAnswer
    {
        [Key]
        public Guid TrtaId { get; set; }
        public Guid TrainingResultId { get; set; }
        public string TrtaAnswer { get; set; }
        public bool TrtaIsCorrect { get; set; }

        public TrainingResult? TrainingResult { get; set; }

        public TrainingResultTypedAnswer(string trtaAnswer)
        {
            TrtaAnswer = trtaAnswer;
        }
    }
}
