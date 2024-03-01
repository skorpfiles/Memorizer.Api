using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class TrainingResultRequest
    {
        public Guid QuestionId { get; set; }
        public DateTime TrainingStartTimeUtc { get; set; }
        public bool IsAnswerCorrect {  get; set; }
        public int AnswerTimeMilliseconds {  get; set; }
    }
}
