using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Net.Http.Headers;
using StackExchange.Redis;

namespace SkorpFiles.Memorizer.Api.Authorization
{
    public class AuthorizationWithCheckTokenMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public AuthorizationWithCheckTokenMiddlewareResultHandler(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Succeeded)
            {
                var accessToken = context.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                var redisDb = _redis.GetDatabase();
                var redisResult = await redisDb.StringGetAsync(new RedisKey(accessToken));
                if (redisResult.ToString() != null)
                {
                    if (redisResult.ToString() != Constants.DisabledManuallyName)
                        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("The token has been deactivated.");
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("The token has not been recognized.");
                }
            }
            else
                await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
