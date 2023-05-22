using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache;
using StackExchange.Redis;

namespace SkorpFiles.Memorizer.Api.Web.Authorization
{
    public class AuthorizationWithCheckTokenMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly ITokenCache _tokenCache;
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public AuthorizationWithCheckTokenMiddlewareResultHandler(ITokenCache tokenCache)
        {
            _tokenCache = tokenCache;
        }

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Succeeded)
            {
                var accessToken = context.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

                var cacheData = await _tokenCache.GetAsync(accessToken);
                if (cacheData != null)
                {
                    if (cacheData!=Constants.DisabledManuallyName)
                    {
                        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
                    }
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
