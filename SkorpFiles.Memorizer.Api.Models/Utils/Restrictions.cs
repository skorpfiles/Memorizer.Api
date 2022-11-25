using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.Utils
{
    public static class Restrictions
    {
        public const int QuestionnaireNameMaxLength = 10000;
        public const int QuestionTextMaxLength = 100000;
        public const int QuestionUntypedAnswerMaxLength = 100000;
        public const int QuestionEstimatedTrainingTimeSecondsMinValue = 1;
        public const int QuestionEstimatedTrainingTimeSecondsMaxValue = 21600;
        public const int LabelNameMaxLength = 10000;
    }
}
