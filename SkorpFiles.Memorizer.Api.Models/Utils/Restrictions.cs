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
        public const int QuestionReferenceMaxLength = 100000;
        public const int QuestionTrainingTimeSecondsMinValue = 1;
        public const int QuestionTrainingTimeSecondsMaxValue = 21600;
        public const int LabelNameMaxLength = 10000;
        public const int TypedAnswerTextMaxLength = 100000;

        public const int InitialQuestionRating = 50;
        public const int MinQuestionRating = 1;
        public const int MaxQuestionRating = 50;
    }
}
