using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache;
using StackExchange.Redis;

namespace SkorpFiles.Memorizer.Api.Web.Authorization
{
    public class AuthorizationWithCheckTokenMiddlewareResultHandler(ITokenCache tokenCache) : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Succeeded)
            {
                var accessToken = context.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

                var cacheData = await tokenCache.GetAsync(accessToken);
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
