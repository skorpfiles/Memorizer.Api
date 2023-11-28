using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal class EntitiesListForRandomChoice<T>:IPickable<T> where T: Entity
    {
        private readonly List<Wrapper<Guid>> _existingIds = new();
        private readonly Dictionary<Guid, T> _existingEntities = new();

        public bool Consumed => !_existingEntities.Any();

        public void Add(T entity)
        {
            if (entity.Id != null)
            {
                _existingEntities.Add(entity.Id.Value, entity);
                _existingIds.Add(new Wrapper<Guid>(entity.Id.Value));
            }
            else
                throw new InvalidOperationException($"Unable to add null entity ID to a {nameof(EntitiesListForRandomChoice<T>)}.");
        }

        public bool Delete(Guid id)
        {
            return _existingIds.RemoveAll(i => i.Value == id) > 0;
        }

        public T Pick(Random random)
        {
            return Pick(random, out _);
        }

        public T PickAndDelete(Random random)
        {
            var result = Pick(random, out Wrapper<Guid> wrapperForPickedId);

            //quick deleting by exchanging this element by the last one in the collection and then deleting the last one.
            wrapperForPickedId.Value = _existingIds[^1].Value;
            _existingIds.RemoveAt(_existingIds.Count - 1);

            return result;
        }

        private T Pick(Random random, out Wrapper<Guid> wrapperForPickedId)
        {
            int index = random.Next(_existingIds.Count - 1);
            wrapperForPickedId = _existingIds[index];
            return _existingEntities[wrapperForPickedId.Value];
        }
    }
}
