using SkorpFiles.Memorizer.Api.Models;
using System.Collections;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal class EntitiesListForWeighedSoftmaxChoice : IPickableTrainingList<GetQuestionsForTrainingResult>, IEnumerable<GetQuestionsForTrainingResult>
    {
        private readonly Dictionary<Guid, GetQuestionsForTrainingResult> _existingEntities = new();
        private static readonly DateTime _minimumMeaningfulTimeForWeightCalculation = new(2020, 1, 1);

        public bool Consumed => !_existingEntities.Any();

        public void Add(GetQuestionsForTrainingResult entity)
        {
            if (entity.Id != null)
            {
                _existingEntities.Add(entity.Id.Value, entity);
            }
            else
                throw new InvalidOperationException($"Unable to add null entity ID to a {nameof(EntitiesListForWeighedSoftmaxChoice)}.");
        }

        public bool Delete(Guid id)
        {
            return _existingEntities.Remove(id);
        }

        public IEnumerator<GetQuestionsForTrainingResult> GetEnumerator()
        {
            foreach (var entity in _existingEntities)
            {
                yield return entity.Value;
            }
        }

        public GetQuestionsForTrainingResult Pick(Random random)
        {
            KeyValuePair<Guid, GetQuestionsForTrainingResult>? pickedKeyValuePair = Utils.SoftmaxSample(_existingEntities, GetItemWeight, random);
            if (pickedKeyValuePair == null)
                throw new InvalidOperationException($"Unable to pick an entity. The existing entities list must not be empty.");
            return pickedKeyValuePair.Value.Value;
        }

        public GetQuestionsForTrainingResult PickAndDelete(Random random)
        {
            KeyValuePair<Guid, GetQuestionsForTrainingResult>? pickedKeyValuePair = Utils.SoftmaxSample(_existingEntities, GetItemWeight, random);
            if (pickedKeyValuePair == null)
                throw new InvalidOperationException($"Unable to pick an entity. The existing entities list must not be empty.");

            if (_existingEntities.Remove(pickedKeyValuePair.Value.Key))
                return pickedKeyValuePair.Value.Value;
            else
                throw new InvalidOperationException($"Unable to remove question from dictionary.");
        }

        public bool Return(GetQuestionsForTrainingResult item)
        {
            bool result;
            if (item.Id != null)
            {
                _existingEntities.TryAdd(item.Id.Value, item);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static int GetItemWeight(GetQuestionsForTrainingResult question)
        {
            int result;
            if (question.LastTrainingTimeUtc == null || question.LastTrainingTimeUtc.Value < _minimumMeaningfulTimeForWeightCalculation)
                result = 0;
            else
                result = (int)Math.Round((question.LastTrainingTimeUtc.Value - _minimumMeaningfulTimeForWeightCalculation).TotalDays);
            return result;
        }
    }
}
