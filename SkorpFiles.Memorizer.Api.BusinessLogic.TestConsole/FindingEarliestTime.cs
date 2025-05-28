namespace SkorpFiles.Memorizer.Api.BusinessLogic.TestConsole
{
    public static class FindingEarliestTime
    {
        public static void TestPerformance(Random random, int itemsCount, int iterations)
        {
            Console.WriteLine("Preparation...");
            var sourceDictionary = new Dictionary<Guid, int>();
            for (int i = 1; i <= itemsCount; i++)
            {
                sourceDictionary.Add(Guid.NewGuid(), random.Next(itemsCount));
            }

            Console.WriteLine("Testing FindingEarliestTime performance...");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                Console.Write($"\rIteration {i + 1} of {iterations}: ");
                var result = sourceDictionary.OrderBy(kvp => kvp.Value).FirstOrDefault();
                Console.WriteLine($"Selected Value: {result.Value}");
            }

            stopwatch.Stop();
            Console.WriteLine($"SoftmaxSample completed {iterations} iterations with {itemsCount} items in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
