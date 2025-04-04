using System.Text.Json.Serialization;

namespace Models.DTOs.AIChat
{
    /// <summary>
    /// يمثل استجابة من خدمة الذكاء الاصطناعي
    /// </summary>
    public class AIQueryResponseDTO
    {
        /// <summary>
        /// معرف الاستجابة
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// نص الاستجابة من الذكاء الاصطناعي
        /// </summary>
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        /// <summary>
        /// النص الأصلي للاستعلام
        /// </summary>
        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// معرف المحادثة
        /// </summary>
        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; } = string.Empty;

        /// <summary>
        /// وقت الإنشاء
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// النموذج المستخدم للإجابة
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// مراجع المستندات المستخدمة في الإجابة
        /// </summary>
        [JsonPropertyName("documentReferences")]
        public List<DocumentReferenceDTO>? DocumentReferences { get; set; }

        /// <summary>
        /// معلومات استخدام الرموز
        /// </summary>
        [JsonPropertyName("tokenUsage")]
        public TokenUsageDTO? TokenUsage { get; set; }
    }

    /// <summary>
    /// يمثل معلومات استخدام الرموز
    /// </summary>
    public class TokenUsageDTO
    {
        /// <summary>
        /// عدد الرموز في الاستعلام
        /// </summary>
        [JsonPropertyName("promptTokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// عدد الرموز في الاستجابة
        /// </summary>
        [JsonPropertyName("completionTokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// إجمالي عدد الرموز المستخدمة
        /// </summary>
        [JsonPropertyName("totalTokens")]
        public int TotalTokens { get; set; }
    }

    /// <summary>
    /// يمثل مرجع لمستند تم استخدامه في الإجابة
    /// </summary>
    public class DocumentReferenceDTO
    {
        /// <summary>
        /// معرف المستند
        /// </summary>
        [JsonPropertyName("documentId")]
        public string DocumentId { get; set; } = string.Empty;

        /// <summary>
        /// اسم المستند
        /// </summary>
        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; } = string.Empty;

        /// <summary>
        /// رقم الصفحة المرجعية
        /// </summary>
        [JsonPropertyName("pageNumber")]
        public int? PageNumber { get; set; }

        /// <summary>
        /// مقتطف من النص المرجعي
        /// </summary>
        [JsonPropertyName("textSnippet")]
        public string? TextSnippet { get; set; }

        /// <summary>
        /// درجة الصلة بالاستعلام (0-1)
        /// </summary>
        [JsonPropertyName("relevanceScore")]
        public double RelevanceScore { get; set; }
    }
} 