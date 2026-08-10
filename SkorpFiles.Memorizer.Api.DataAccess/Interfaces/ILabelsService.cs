namespace SkorpFiles.Memorizer.Api.DataAccess.Interfaces
{
    public interface ILabelsService
    {
        Task<Dictionary<string, Guid>> EnsureLabelsAsync(
            IEnumerable<string> requestedNames,
            CancellationToken cancellationToken = default);
    }
}
