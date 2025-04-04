using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models.DTOs.AIChat
{
    /// <summary>
    /// نموذج طلب استعلام الذكاء الاصطناعي
    /// </summary>
    public class AIQueryRequestDTO
    {
        /// <summary>
        /// الاستعلام المطلوب
        /// </summary>
        [Required(ErrorMessage = "الاستعلام مطلوب")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "يجب أن يكون الاستعلام بين 3 و 1000 حرف")]
        public string Query { get; set; }

        /// <summary>
        /// معرف المحادثة (اختياري)
        /// </summary>
        [StringLength(50, ErrorMessage = "يجب ألا يتجاوز معرف المحادثة 50 حرفًا")]
        public string ConversationId { get; set; }

        /// <summary>
        /// قائمة بمعرفات الملفات المرتبطة بهذا الاستعلام
        /// </summary>
        [JsonPropertyName("attachedFileIds")]
        public List<string>? AttachedFileIds { get; set; }

        /// <summary>
        /// إعدادات النموذج اللغوي
        /// </summary>
        [JsonPropertyName("modelSettings")]
        public AIModelSettingsDTO? ModelSettings { get; set; }

        /// <summary>
        /// عنوان IP للمستخدم
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string? IpAddress { get; set; }

        /// <summary>
        /// معلومات متصفح المستخدم
        /// </summary>
        [JsonPropertyName("userAgent")]
        public string? UserAgent { get; set; }
    }

    /// <summary>
    /// إعدادات النموذج اللغوي
    /// </summary>
    public class AIModelSettingsDTO
    {
        /// <summary>
        /// اسم النموذج المستخدم
        /// </summary>
        [JsonPropertyName("modelName")]
        public string ModelName { get; set; } = "deepseek-r1-thinking";

        /// <summary>
        /// درجة حرارة النموذج (للإبداعية)
        /// </summary>
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// الحد الأقصى لعدد الرموز في الاستجابة
        /// </summary>
        [JsonPropertyName("maxTokens")]
        public int MaxTokens { get; set; } = 2000;
    }
} 