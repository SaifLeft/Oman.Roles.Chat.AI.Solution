using System.Text.Json.Serialization;

namespace Models.DTOs.Authorization
{
    public class RoleDTO
    {
        /// <summary>
        /// معرف الدور
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// اسم الدور
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// الأذونات المرتبطة
        /// </summary>
        [JsonPropertyName("permissions")]
        public List<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// وصف الدور
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}