using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training.MakingListStrategies.WeightedRandomSamplingStrategy
{
    internal class WeightedRandomSamplingPicker<T> : IPickableTrainingList<T>
    {
        private readonly Func<T, int> _weightSelector;
        private readonly PriorityQueue<T, double> _queue;
        private readonly Random _random;
        private readonly double _alpha;

        public WeightedRandomSamplingPicker(IEnumerable<T> items, Func<T, int> weightSelector, Random random, double alpha = 1)
        {
            _weightSelector = weightSelector ?? throw new ArgumentNullException(nameof(weightSelector));
            _queue = new PriorityQueue<T, double>();
            _random = random;

            if (random is null)
                throw new ArgumentNullException(nameof(random));

            _alpha = alpha;

            foreach (var item in items)
            {
                Enqueue(item, random);
            }
        }

        public bool Consumed => _queue.Count == 0;

        public T Pick(Random random)
        {
            if (_queue.Count == 0)
                throw new InvalidOperationException("All items have been consumed.");

            return _queue.Peek();
        }

        public T PickAndDelete(Random random)
        {
            if (_queue.Count == 0)
                throw new InvalidOperationException("All items have been consumed.");

            return _queue.Dequeue();
        }

        public bool Return(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            Enqueue(item, _random);
            return true;
        }

        private void Enqueue(T item, Random random)
        {
            int weight = _weightSelector(item);
            if (weight < 1)
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be >= 1.");

            double effective = (_alpha == 1.0) ? weight : Math.Pow(weight, _alpha);

            double u;
            do
            {
                u = random.NextDouble();
            } while (u <= 0.0);

            double key = -Math.Log(u) / effective;
            _queue.Enqueue(item, key);
        }
    }
}
