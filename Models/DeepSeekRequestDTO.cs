using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// نموذج طلب DeepSeek
    /// </summary>
    public class DeepSeekRequestDTO
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<ChatMessageDTO> Messages { get; set; } = new List<ChatMessageDTO>();

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 2000;
    }

    /// <summary>
    /// نموذج استجابة DeepSeek
    /// </summary>
    public class DeepSeekResponseDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("choices")]
        public List<DeepSeekChoiceDTO> Choices { get; set; } = new List<DeepSeekChoiceDTO>();

        [JsonPropertyName("usage")]
        public DeepSeekUsageDTO Usage { get; set; } = new DeepSeekUsageDTO();
    }

    /// <summary>
    /// خيارات استجابة DeepSeek
    /// </summary>
    public class DeepSeekChoiceDTO
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public ChatMessageDTO Message { get; set; } = new ChatMessageDTO();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// استخدام توكنز DeepSeek
    /// </summary>
    public class DeepSeekUsageDTO
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
