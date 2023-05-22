using StackExchange.Redis;

namespace SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache
{
    public class RedisTokenCache : ITokenCache
    {
        private IConnectionMultiplexer? _redis;
        public async Task<string> GetAsync(string key)
        {
            var database = _redis!.GetDatabase();
            return await database.StringGetAsync(new RedisKey(key));
        }

        public void Initialize(object path)
        {
            _redis = ConnectionMultiplexer.Connect((string)path);
        }

        public async Task<bool> SetAsync(string key, string value)
        {
            var database = _redis!.GetDatabase();
            return await database.StringSetAsync(new RedisKey(key),new RedisValue(value));
        }
    }
}
