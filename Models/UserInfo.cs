using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// معلومات المستخدم
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// اسم المستخدم
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الكامل
        /// </summary>
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// أدوار المستخدم
        /// </summary>
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// نموذج طلب تسجيل الدخول
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// اسم المستخدم
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// كلمة المرور
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج استجابة تسجيل الدخول
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// رمز الوصول (JWT)
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// رمز التحديث
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ انتهاء الصلاحية
        /// </summary>
        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// معلومات المستخدم
        /// </summary>
        [JsonPropertyName("user")]
        public UserInfo User { get; set; } = new UserInfo();
    }

    /// <summary>
    /// نموذج طلب تحديث الرمز
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// رمز التحديث
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }


}