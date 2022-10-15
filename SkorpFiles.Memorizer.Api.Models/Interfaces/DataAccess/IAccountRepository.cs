namespace SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess
{
    public interface IAccountRepository
    {
        Task RegisterUserActivityAsync(string userName, string userId);
    }
}
