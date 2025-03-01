using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public interface IJwtService
    {
        /// <summary>
        /// إنشاء رمز JWT
        /// </summary>
        /// <param name="user">معلومات المستخدم</param>
        /// <returns>معلومات الرمز المنشأ</returns>
        LoginResponse GenerateJwtToken(UserInfo user);

        /// <summary>
        /// التحقق من صحة الرمز وقراءة البيانات منه
        /// </summary>
        /// <param name="token">الرمز المراد التحقق منه</param>
        /// <returns>معلومات المستخدم إذا كان الرمز صالحًا، أو null إذا كان الرمز غير صالح</returns>
        UserInfo? ValidateJwtToken(string token);

        /// <summary>
        /// تحديث الرمز باستخدام رمز التحديث
        /// </summary>
        /// <param name="refreshToken">رمز التحديث</param>
        /// <returns>معلومات الرمز الجديد، أو null إذا كان رمز التحديث غير صالح</returns>
        LoginResponse? RefreshToken(string refreshToken);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _refreshTokens = new();

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// إنشاء رمز JWT
        /// </summary>
        public LoginResponse GenerateJwtToken(UserInfo user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"));

            var tokenExpiryMinutes = int.Parse(jwtSettings["TokenExpiryMinutes"] ?? "60");
            var expiryTime = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // إضافة الأدوار كـ claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryTime,
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // إنشاء رمز التحديث
            var refreshToken = GenerateRefreshToken();
            _refreshTokens[refreshToken] = user.Id;

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiryTime,
                User = user
            };
        }

        /// <summary>
        /// التحقق من صحة الرمز وقراءة البيانات منه
        /// </summary>
        public UserInfo? ValidateJwtToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // التحقق من صحة الرمز
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // استخراج البيانات من الرمز
                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                var email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                var roles = jwtToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

                return new UserInfo
                {
                    Id = userId,
                    Email = email,
                    Username = username,
                    Roles = roles
                };
            }
            catch
            {
                // إذا حدث خطأ أثناء التحقق من الرمز، فإنه غير صالح
                return null;
            }
        }

        /// <summary>
        /// تحديث الرمز باستخدام رمز التحديث
        /// </summary>
        public LoginResponse? RefreshToken(string refreshToken)
        {
            // التحقق مما إذا كان رمز التحديث موجودًا
            if (!_refreshTokens.TryGetValue(refreshToken, out var userId))
                return null;

            // في بيئة حقيقية، ستقوم بالبحث عن المستخدم في قاعدة البيانات
            // هنا نقوم بإنشاء مستخدم افتراضي لأغراض التوضيح
            var user = new UserInfo
            {
                Id = userId,
                Username = "user123",
                Email = "user@example.com",
                FullName = "مستخدم نموذجي",
                Roles = new List<string> { "User" }
            };

            // إنشاء رمز جديد
            var response = GenerateJwtToken(user);

            // إزالة رمز التحديث القديم
            _refreshTokens.Remove(refreshToken);

            return response;
        }

        /// <summary>
        /// إنشاء رمز تحديث عشوائي
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}