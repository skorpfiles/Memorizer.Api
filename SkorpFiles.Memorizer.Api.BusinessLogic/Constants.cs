using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public static class Constants
    {
        public const double AllowableErrorFraction = 0.1;

        public const int StandardTrainingTimeSeconds = 10;

        public const double NewQuestionsLearningTimeMultiplicator = 2;

        public const string SumOfFractionsCannotBeMoreThan1Message = "New questions fraction and penalty questions fraction cannot be negative.";
        public const string NegativeFractionsMessage = "New questions fraction and penalty questions fraction cannot be more than 100% in total.";
        public const string NonPositiveLengthValueMessage = "Length value must be positive.";
    }
}
