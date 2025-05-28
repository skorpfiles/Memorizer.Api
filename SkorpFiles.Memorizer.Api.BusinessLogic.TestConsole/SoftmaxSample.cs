using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.TestConsole
{
    public static class SoftmaxSample
    {
        public static void TestPerformance(Random random, int itemsCount, int iterations)
        {
            Console.WriteLine("Preparation...");
            var sourceDictionary = new Dictionary<Guid, int>();
            for(int i=1;i<= itemsCount; i++)
            {
                sourceDictionary.Add(Guid.NewGuid(), random.Next(itemsCount));
            }

            Console.WriteLine("Testing SoftmaxSample performance...");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                Console.Write($"\rIteration {i + 1} of {iterations}: ");
                var result = Utils.SoftmaxSample(sourceDictionary, value => value, random, 1.0);
                if (result.HasValue)
                {
                    Console.WriteLine($"Selected Value: {result.Value.Value}");
                }
                else
                {
                    Console.WriteLine("No item selected.");
                    break;
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"SoftmaxSample completed {iterations} iterations with {itemsCount} items in {stopwatch.ElapsedMilliseconds} ms.");
        }

        public static void TestCorrectness(Random random, int itemsCount)
        {
            Console.WriteLine("Preparation...");
            var sourceDictionary = new Dictionary<Guid, int>();
            for (int i = 1; i <= itemsCount; i++)
            {
                sourceDictionary.Add(Guid.NewGuid(), random.Next(itemsCount));
            }

            while(sourceDictionary.Count > 0)
            {
                Console.WriteLine("Source Dictionary:");
                foreach (var kvp in sourceDictionary)
                {
                    Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                }

                var realMaximum = sourceDictionary.Values.Max();
                Console.WriteLine($"Real Maximum Value: {realMaximum}");
                var softmaxResult = Utils.SoftmaxSample(sourceDictionary, value => value, random, 1.0);

                if (softmaxResult.HasValue)
                {
                    Console.WriteLine($"Softmax Value: {softmaxResult.Value.Value}");
                    sourceDictionary.Remove(softmaxResult.Value.Key);
                }
                else
                {
                    Console.WriteLine("No item selected.");
                    break;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
