using System.Security.Claims;

namespace SkorpFiles.Memorizer.Api.Web.Authorization
{
    public interface ITokenHelper
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}
