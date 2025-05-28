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
            var entitiesList = new Training.EntitiesListForWeighedSoftmaxChoice();
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
    }
}
