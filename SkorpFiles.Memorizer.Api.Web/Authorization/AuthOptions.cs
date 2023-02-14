using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SkorpFiles.Memorizer.Api.Web.Authorization
{
    public static class AuthOptions
    {
        public const string Issuer = "MyAuthServer"; // издатель токена
        public const string Audience = "MyAuthClient"; // потребитель токена
        public const int AccessTokenLifetime = 1000;
        public const int RefreshTokenLifetime = 43200;

        public static SymmetricSecurityKey GetSymmetricSecurityKey(IConfiguration configuration)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["AuthenticationTokenCipherKey"]));
        }
    }
}
