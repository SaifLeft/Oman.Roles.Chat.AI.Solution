using System.Text.Json.Serialization;

namespace Models.DTOs.AIChat
{
    /// <summary>
    /// نموذج طلب DeepSeek
    /// </summary>
    public class DeepSeekRequestDTO
    {
        /// <summary>
        /// المطالبة أو الاستعلام المرسل
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// نص السياق (اختياري)
        /// </summary>
        [JsonPropertyName("context")]
        public string? Context { get; set; }

        /// <summary>
        /// نموذج اللغة المراد استخدامه
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; } = "deepseek-chat";

        /// <summary>
        /// نظام المطالبة
        /// </summary>
        [JsonPropertyName("system_prompt")]
        public string SystemPrompt { get; set; } = string.Empty;

        /// <summary>
        /// الحد الأقصى للرموز في الإجابة
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 4000;

        /// <summary>
        /// عامل الإبداع
        /// </summary>
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;
    }

    /// <summary>
    /// نموذج استجابة DeepSeek
    /// </summary>
    public class DeepSeekResponseDTO
    {
        /// <summary>
        /// المعرف الفريد للطلب
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// النص الناتج من الاستجابة
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// هل تم إكمال الاستجابة بنجاح
        /// </summary>
        [JsonPropertyName("finished")]
        public bool Finished { get; set; }
    }
} 