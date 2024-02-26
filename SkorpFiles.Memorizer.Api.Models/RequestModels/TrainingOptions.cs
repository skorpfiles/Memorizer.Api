using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class TrainingOptions
    {
        public TrainingLengthType LengthType { get; set; }
        public int LengthValue { get; set; } //for time - seconds
        public double NewQuestionsFraction {  get; set; }
        public double PrioritizedPenaltyQuestionsFraction {  get; set; }
    }
}
