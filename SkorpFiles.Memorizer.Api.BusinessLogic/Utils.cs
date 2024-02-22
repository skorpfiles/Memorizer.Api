using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public static class Utils
    {
        public static List<Question> FindBestQuestionsTimesCombination(List<Question> questions, int target)
        {
            if (questions==null)
                throw new ArgumentNullException(nameof(questions));
            if (target < 0)
                throw new ArgumentException($"{nameof(target)} should be positive.");

            int n = questions.Count;
            int[,] dp = new int[n + 1, target + 1];

            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= target; j++)
                {
                    if (i == 0 || j == 0)
                        dp[i, j] = 0;
                    else if (GetFullEstimatedTimeOfQuestion(questions[i - 1]) <= j)
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i - 1, j - GetFullEstimatedTimeOfQuestion(questions[i - 1])] + GetFullEstimatedTimeOfQuestion(questions[i - 1]));
                    else
                        dp[i, j] = dp[i - 1, j];
                }
            }

            List<Question> result = new();
            int res = target;
            for (int i = n; i > 0 && res > 0; i--)
            {
                if (dp[i, res] != dp[i - 1, res])
                {
                    result.Add(questions[i - 1]);
                    res -= GetFullEstimatedTimeOfQuestion(questions[i - 1]);
                }
            }

            return result;
        }

        public static int GetFullEstimatedTimeOfQuestion(Question question)
        {
            int result;
            if (!question.MyStatus!.IsNew)
            {
                result = question.EstimatedTrainingTimeSeconds;
            }
            else
            {
                result = (int)Math.Round(question.EstimatedTrainingTimeSeconds * Constants.NewQuestionsLearningTimeMultiplicator);
            }
            return result;
        }
    }
}
