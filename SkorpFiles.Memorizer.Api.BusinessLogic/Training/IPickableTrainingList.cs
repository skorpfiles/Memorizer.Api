namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal interface IPickableTrainingList<T>
    {
        bool Consumed { get; }
        T Pick(Random random);
        T PickAndDelete(Random random);
        bool Return(T item);
    }
}
