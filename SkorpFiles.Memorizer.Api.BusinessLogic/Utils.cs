using SkorpFiles.Memorizer.Api.BusinessLogic.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
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
        public static List<GetQuestionsForTrainingResult> FindBestQuestionsTimesCombination(List<GetQuestionsForTrainingResult> questions, int target)
        {
            ArgumentNullException.ThrowIfNull(questions);
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
                    else if (questions[i - 1].FullActualTrainingTimeSeconds() <= j)
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i - 1, j - questions[i - 1].FullActualTrainingTimeSeconds()] + questions[i - 1].FullActualTrainingTimeSeconds());
                    else
                        dp[i, j] = dp[i - 1, j];
                }
            }

            List<GetQuestionsForTrainingResult> result = [];
            int res = target;
            for (int i = n; i > 0 && res > 0; i--)
            {
                if (dp[i, res] != dp[i - 1, res])
                {
                    result.Add(questions[i - 1]);
                    res -= questions[i - 1].FullActualTrainingTimeSeconds();
                }
            }

            return result;
        }

        public static KeyValuePair<TKey, TValue>? SoftmaxSample<TKey, TValue>(IDictionary<TKey, TValue> elements, Func<TValue,int> weightCalculationFunc, Random random, double temperature = 1.0) where TKey:notnull
        {
            ArgumentNullException.ThrowIfNull(elements);

            if (!elements.Any())
            {
                return default;
            }

            int minWeight = elements.Values.Min(weightCalculationFunc);

            var expList = elements
                .Select(e => (item: e, exp: Math.Exp(NormalizeWeight(weightCalculationFunc, e.Value, minWeight, temperature))))
                .ToList();

            double total = expList.Sum(e => e.exp);
            double r = random.NextDouble() * total;

            foreach (var (item, exp) in expList)
            {
                r -= exp;
                if (r <= 0)
                {
                    return item;
                }
            }

            return expList.Last().item;
        }

        private static double NormalizeWeight<T>(Func<T, int> weightCalculationFunc, T value, int minWeight, double temperature)
        {
            const double maxPossibleWeight = 709.78; // Maximum value for exp(x) to avoid overflow in double precision
            double weight = (weightCalculationFunc(value) - minWeight) / temperature;
            if (weight > maxPossibleWeight)
            {
                weight = maxPossibleWeight;
            }
            return weight;
        }
    }
}
