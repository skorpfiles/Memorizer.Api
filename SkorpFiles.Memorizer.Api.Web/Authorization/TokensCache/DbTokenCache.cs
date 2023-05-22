using SkorpFiles.Memorizer.Api.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;

namespace SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache
{
    public class DbTokenCache : ITokenCache
    {
        private IAccountLogic _accountLogic;

        public DbTokenCache(IAccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _accountLogic.GetTokenInfoFromCacheAsync(key);
        }

        public void Initialize(object path)
        {
            
        }

        public async Task<bool> SetAsync(string key, string value)
        {
            try
            {
                await _accountLogic.SetTokenToCacheAsync(key, value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
