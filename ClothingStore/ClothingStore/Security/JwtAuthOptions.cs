using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ClothingStore.Security
{
    public class JwtAuthOptions
    {
        public const string ISSUER = "ClothingStore";
        public const string AUDIENCE = "Client";
        const string KEY = "some_super_ultra_mega_giga_secret_key_667";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
