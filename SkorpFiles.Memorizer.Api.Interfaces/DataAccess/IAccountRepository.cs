namespace SkorpFiles.Memorizer.Api.Interfaces.DataAccess
{
    public interface IAccountRepository
    {
        Task RegisterUserActivityAsync(string userName, string userId);
    }
}
