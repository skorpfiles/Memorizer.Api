using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SkorpFiles.Memorizer.Api.Web.Authorization
{
    public static class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        public const int LIFETIME = 1000; // время жизни токена в минутах
        public static SymmetricSecurityKey GetSymmetricSecurityKey(IConfiguration configuration)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["AuthenticationTokenCipherKey"]));
        }
    }
}
