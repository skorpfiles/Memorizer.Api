namespace SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic
{
    public interface IAccountLogic
    {
        Task RegisterUserActivityAsync(string userName, string userId);

        Task SetTokenToCacheAsync(string key, string value);

        Task<string?> GetTokenInfoFromCacheAsync(string key);
    }
}
