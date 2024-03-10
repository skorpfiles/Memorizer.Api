using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SkorpFiles.Memorizer.Api.Web.Authorization
{
    public static class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        public const int LIFETIME = 1000;
        public static SymmetricSecurityKey GetSymmetricSecurityKey(byte[] key)
        {
            return new SymmetricSecurityKey(key);
        }
    }
}
