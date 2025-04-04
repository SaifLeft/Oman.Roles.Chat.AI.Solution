using System.Text.Json.Serialization;

namespace Models.DTOs.Authorization
{
    /// <summary>
    /// استجابة المصادقة
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// رمز الوصول
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// رمز التحديث
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        public AuthResponse(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
} 