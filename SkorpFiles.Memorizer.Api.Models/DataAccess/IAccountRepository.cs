namespace SkorpFiles.Memorizer.Api.Models.DataAccess
{
    public interface IAccountRepository
    {
        Task RegisterUserActivityAsync(string userName, string userId);
    }
}
