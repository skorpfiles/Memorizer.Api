namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal interface IEditableListWithId<TItem, TId>
    {
        void Add(TItem item);
        bool Delete(TId id);
    }
}
