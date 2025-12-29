namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal interface IPickableTrainingList<T>
    {
        /// <summary>
        /// Indicates whether all items have been consumed from the list.
        /// </summary>
        bool Consumed { get; }
        /// <summary>
        /// Selects and returns a value of type T at random using the specified random number generator, but does not remove it from the collection.
        /// </summary>
        /// <param name="random">The random number generator to use when selecting the value. Cannot be null.</param>
        /// <returns>A randomly selected value of type T.</returns>
        T Pick(Random random);
        /// <summary>
        /// Selects and removes a random element from the collection.
        /// </summary>
        /// <param name="random">The random number generator to use when selecting the element. Cannot be null.</param>
        /// <returns>The element that was randomly selected and removed from the collection.</returns>
        T PickAndDelete(Random random);
        /// <summary>
        /// Returns an item to the pool for future reuse.
        /// </summary>
        /// <param name="item">The item to return to the pool. Must not be null.</param>
        /// <returns>true if the item was successfully returned to the pool; otherwise, false.</returns>
        bool Return(T item);
    }
}
