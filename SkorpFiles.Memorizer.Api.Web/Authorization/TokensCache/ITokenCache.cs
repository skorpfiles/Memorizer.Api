namespace SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache
{
    public interface ITokenCache
    {
        void Initialize(object path);
        Task<bool> SetAsync(string key, string value);
        Task<string?> GetAsync(string key);
    }
}
