using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// استجابة الرمز المميز من Google
    /// </summary>
    public class GoogleTokenResponse
    {
        /// <summary>
        /// رمز الوصول
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// مدة صلاحية الرمز بالثانية
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// رمز التحديث
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// نوع الرمز
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        /// <summary>
        /// رمز الهوية
        /// </summary>
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// معلومات المستخدم من Google
    /// </summary>
    public class GoogleUserInfo
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// هل تم التحقق من البريد الإلكتروني
        /// </summary>
        [JsonPropertyName("verified_email")]
        public bool VerifiedEmail { get; set; }

        /// <summary>
        /// الاسم الكامل
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الأول
        /// </summary>
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// اسم العائلة
        /// </summary>
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = string.Empty;

        /// <summary>
        /// رابط الصورة الشخصية
        /// </summary>
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        /// <summary>
        /// اللغة المفضلة
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج طلب تسجيل الدخول باستخدام Google
    /// </summary>
    public class GoogleSignInRequest
    {
        /// <summary>
        /// رمز المصادقة الذي تم الحصول عليه من Google
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// رابط إعادة التوجيه الذي تم استخدامه في طلب المصادقة
        /// </summary>
        [JsonPropertyName("redirectUri")]
        public string RedirectUri { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج طلب التحقق من صحة الرمز
    /// </summary>
    public class ValidateTokenRequest
    {
        /// <summary>
        /// الرمز المراد التحقق منه
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}