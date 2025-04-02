using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Authorization;
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
        Task<LoginResponse> GenerateJwtToken(string Id);

        /// <summary>
        /// التحقق من صحة الرمز وقراءة البيانات منه
        /// </summary>
        /// <param name="token">الرمز المراد التحقق منه</param>
        /// <returns>معلومات المستخدم إذا كان الرمز صالحًا، أو null إذا كان الرمز غير صالح</returns>
        UserDTO? ValidateJwtToken(string token);

        /// <summary>
        /// تحديث الرمز باستخدام رمز التحديث
        /// </summary>
        /// <param name="refreshToken">رمز التحديث</param>
        /// <returns>معلومات الرمز الجديد، أو null إذا كان رمز التحديث غير صالح</returns>
        Task<LoginResponse?> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// إبطال رمز التحديث
        /// </summary>
        /// <param name="refreshToken">رمز التحديث</param>
        /// <returns>نجاح العملية</returns>
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// إبطال جميع رموز التحديث للمستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <returns>نجاح العملية</returns>
        Task<bool> RevokeAllUserRefreshTokensAsync(long userId);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly MuhamiContext _context;

        public JwtService(IConfiguration configuration, MuhamiContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// إنشاء رمز JWT
        /// </summary>
        public async Task<LoginResponse> GenerateJwtToken(string Id)
        {
            // get User from database to get the latest roles

            // حفظ رمز التحديث في قاعدة البيانات
            if (!long.TryParse(Id, out long userIdLong))
            {
                throw new InvalidOperationException("Invalid user ID format");
            }

            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userIdLong);

            if (userEntity == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var Roles = new List<string> { userEntity.UserRole };
            string Email = userEntity.Email;
            string Username = userEntity.Username;




            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"));

            var tokenExpiryMinutes = int.Parse(jwtSettings["TokenExpiryMinutes"] ?? "60");
            var expiryTime = DateTime.Now.AddMinutes(tokenExpiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userIdLong.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, Username)
            };

            // إضافة الأدوار كـ claims
            foreach (var role in Roles)
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

            // تحديد تاريخ انتهاء رمز التحديث
            var refreshTokenExpiryDays = int.Parse(jwtSettings["RefreshTokenExpiryDays"] ?? "7");
            var refreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenExpiryDays);



            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = userIdLong,
                ExpiresAt = refreshTokenExpiryTime,
                CreatedByUserId = userIdLong,
                CreateDate = DateTime.Now
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiryTime,
            };
        }

        /// <summary>
        /// التحقق من صحة الرمز وقراءة البيانات منه
        /// </summary>
        public UserDTO? ValidateJwtToken(string token)
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
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                var roles = jwtToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

                return new UserDTO
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
        public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // البحث عن رمز التحديث في قاعدة البيانات
                var tokenEntity = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken &&
                                               rt.RevokedAt == null &&
                                               rt.IsDeleted != true);

                if (tokenEntity == null)
                {
                    // رمز التحديث غير موجود أو ملغي
                    return null;
                }

                // التحقق من صلاحية رمز التحديث
                if (tokenEntity.ExpiresAt < DateTime.Now)
                {
                    // تعليم الرمز كملغي لأنه منتهي الصلاحية
                    tokenEntity.RevokedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return null;
                }

                // إنشاء معلومات المستخدم من بيانات المستخدم
                var user = tokenEntity.User;
                if (user == null || user.IsDeleted == true || !user.IsActive)
                {
                    // المستخدم غير موجود أو غير نشط
                    return null;
                }


                // إلغاء رمز التحديث الحالي
                tokenEntity.RevokedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // إنشاء رمز جديد
                return await GenerateJwtToken(user.Id.ToString());
            }
            catch (Exception)
            {
                // في حالة حدوث أي خطأ، نعيد null
                return null;
            }
        }

        /// <summary>
        /// إبطال رمز التحديث
        /// </summary>
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var tokenEntity = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken &&
                                               rt.RevokedAt == null &&
                                               rt.IsDeleted != true);

                if (tokenEntity == null)
                {
                    // رمز التحديث غير موجود أو ملغي بالفعل
                    return false;
                }

                tokenEntity.RevokedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// إبطال جميع رموز التحديث للمستخدم
        /// </summary>
        public async Task<bool> RevokeAllUserRefreshTokensAsync(long userId)
        {
            try
            {
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId &&
                                 rt.RevokedAt == null &&
                                 rt.IsDeleted != true)
                    .ToListAsync();

                if (!tokens.Any())
                {
                    // لا توجد رموز تحديث نشطة للمستخدم
                    return true;
                }

                foreach (var token in tokens)
                {
                    token.RevokedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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