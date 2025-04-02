using System.Text.Json.Serialization;

namespace Models.DTOs.Authorization
{
    public class AdminUpdateUserRequestDTO
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

        /// <summary>
        /// رقم الهاتف
        /// </summary>
        public long? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
