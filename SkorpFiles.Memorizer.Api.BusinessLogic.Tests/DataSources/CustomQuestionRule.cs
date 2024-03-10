using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    internal class CustomQuestionRule
    {
        public CustomQuestionRule(int exactValue)
        {
            IsExactValue = true;
            ExactValue = exactValue;
        }

        public CustomQuestionRule(int minValue, int maxValue)
        {
            IsExactValue = false;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is CustomQuestionRule customQuestionRuleObj)
            {
                return IsExactValue.Equals(customQuestionRuleObj.IsExactValue) && ExactValue.Equals(customQuestionRuleObj.ExactValue) && MinValue.Equals(customQuestionRuleObj.MinValue) && MaxValue.Equals(customQuestionRuleObj.MaxValue);
            }
            else { return false; }
        }

        public bool IsExactValue;
        public int ExactValue;
        public int MinValue;
        public int MaxValue;

        public override int GetHashCode()
        {
            return HashCode.Combine(IsExactValue, ExactValue, MinValue, MaxValue);
        }
    }
}
