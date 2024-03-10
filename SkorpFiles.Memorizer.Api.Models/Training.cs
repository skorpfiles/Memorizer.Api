using SkorpFiles.Memorizer.Api.Models.Abstract;
using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class Training : Entity
    {
        public string? Name { get; set; }
        public DateTime LastTimeUtc { get; set; }
        public TrainingLengthType LengthType { get; set; }
        public int QuestionsCount { get; set; }
        public int TimeMinutes { get; set; }
        public decimal NewQuestionsFraction {  get; set; }
        public decimal PenaltyQuestionsFraction {  get; set; }
        public List<Questionnaire>? Questionnaires {  get; set; } 
    }
}
