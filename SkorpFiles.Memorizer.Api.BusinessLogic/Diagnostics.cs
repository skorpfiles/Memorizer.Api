using SkorpFiles.Memorizer.Api.BusinessLogic.Training.MakingListStrategies.WeightedRandomSamplingStrategy;
using System.Diagnostics;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public static class Diagnostics
    {
        public static void TestEntitiesListForRandomChoice(Random random, int itemsCount, int iterations)
        {
            Console.WriteLine("Preparing entities list for random choice...");
            var entitiesList = new Training.EntitiesListForRandomChoice<Models.GetQuestionsForTrainingResult>();
            for (int i = 0; i < itemsCount; i++)
            {
                var entity = new Models.GetQuestionsForTrainingResult
                {
                    Id = Guid.NewGuid(),
                    QuestionText = $"Question {i + 1}",
                    LastTrainingTimeUtc = DateTime.UtcNow.AddDays(random.Next(-10000, 10000)), // Random last training time
                };
                entitiesList.Add(entity);
            }

            Console.WriteLine("Test PickAndDelete method performance...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                Console.Write($"\rIteration {i + 1} of {iterations}: ");
                var picked = entitiesList.PickAndDelete(random);
                if (picked == null)
                {
                    Console.WriteLine("No more entities to pick.");
                    break;
                }
                else
                {
                    Console.WriteLine($"Picked: {picked.LastTrainingTimeUtc}");
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Time taken for {iterations} iterations: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void TestEntitiesListForWeighedSoftmaxChoice(Random random, int itemsCount, int iterations)
        {
            Console.WriteLine("Preparing entities list for weighed softmax choice...");
            var entitiesList = new Training.MakingListStrategies.Strategy2018.EntitiesListForWeighedSoftmaxChoice();
            for (int i = 0; i < itemsCount; i++)
            {
                var entity = new Models.GetQuestionsForTrainingResult
                {
                    Id = Guid.NewGuid(),
                    QuestionText = $"Question {i + 1}",
                    LastTrainingTimeUtc = DateTime.UtcNow.AddDays(random.Next(-10000, 10000)), // Random last training time
                };
                entitiesList.Add(entity);
            }
            Console.WriteLine("Test PickAndDelete method performance...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                Console.Write($"\rIteration {i + 1} of {iterations}: ");
                var picked = entitiesList.PickAndDelete(random);
                if (picked == null)
                {
                    Console.WriteLine("No more entities to pick.");
                    break;
                }
                else
                {
                    Console.WriteLine($"Picked: {picked.LastTrainingTimeUtc}");
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Time taken for {iterations} iterations: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void TestEntitiesListForWeightedRandomSamplingStrategy(Random random, int itemsCount, int iterations, double alpha)
        {
            Console.WriteLine("Preparing entities list for weighted random sampling strategy...");
            var entitiesList = new List<Models.GetQuestionsForTrainingResult>();

            var entitiesForRating = itemsCount / 50;
            for (int r = 1; r <= 50; r++)
            {
                for (int i = 0; i < entitiesForRating; i++)
                {
                    var entity = new Models.GetQuestionsForTrainingResult
                    {
                        Id = Guid.NewGuid(),
                        QuestionText = $"Question Rating {r} - {i + 1}",
                        LastTrainingTimeUtc = DateTime.UtcNow,
                        QuestionUserRating = r
                    };
                    Console.WriteLine($"Prepared entity Id: {entity.Id}, weight: {entity.QuestionUserRating}");
                    entitiesList.Add(entity);
                }
            }

            var picker = new WeightedRandomSamplingPicker<Models.GetQuestionsForTrainingResult>(entitiesList, (item) => item.QuestionUserRating ?? 50, random, alpha);

            Console.WriteLine("Test PickAndDelete method performance...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                Console.Write($"\rIteration {i + 1} of {iterations}: ");
                var picked = picker.PickAndDelete(random);
                if (picked == null)
                {
                    Console.WriteLine("No more entities to pick.");
                    break;
                }
                else
                {
                    Console.WriteLine($"Picked Id: {picked.Id}, weight: {picked.QuestionUserRating}");
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Time taken for {iterations} iterations: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
