using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class TrainingResult
    {
        public Guid QuestionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RecordingTime { get; set; }
        public bool IsAnswerCorrect {  get; set; }
        public List<GivenTypedAnswer>? GivenTypedAnswers { get; set; }
        public int AnswerTimeMilliseconds {  get; set; }
        public QuestionStatus? InitialQuestionStatus {  get; set; }
        public QuestionStatus? ResultQuestionStatus { get; set;}
    }
}
