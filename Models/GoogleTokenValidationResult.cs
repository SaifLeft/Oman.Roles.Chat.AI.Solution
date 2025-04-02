using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// نتيجة التحقق من رمز Google
    /// </summary>
    public class GoogleTokenValidationResult
    {
        /// <summary>
        /// هل الرمز صالح
        /// </summary>
        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// معرف المستخدم في Google
        /// </summary>
        [JsonPropertyName("sub")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني للمستخدم
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// هل تم التحقق من البريد الإلكتروني
        /// </summary>
        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

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
        /// الاسم الكامل
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// صورة المستخدم
        /// </summary>
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        /// <summary>
        /// اللغة المفضلة
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;
    }
}