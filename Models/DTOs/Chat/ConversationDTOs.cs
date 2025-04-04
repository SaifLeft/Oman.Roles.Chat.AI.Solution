using System.Text.Json.Serialization;

namespace Models.DTOs.Chat
{
    /// <summary>
    /// نموذج طلب الاستعلام من الذكاء الاصطناعي
    /// </summary>
    public class AIQueryRequestDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; } = string.Empty;

        /// <summary>
        /// نص الاستعلام
        /// </summary>
        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// معرفات الملفات المرتبطة (اختياري)
        /// </summary>
        [JsonPropertyName("fileIds")]
        public List<string>? FileIds { get; set; }
    }

    /// <summary>
    /// نموذج استجابة الذكاء الاصطناعي
    /// </summary>
    public class AIQueryResponseDTO
    {
        /// <summary>
        /// نص الرد
        /// </summary>
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        /// <summary>
        /// معرف المحادثة
        /// </summary>
        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; } = string.Empty;

        /// <summary>
        /// مصادر الرد
        /// </summary>
        [JsonPropertyName("sources")]
        public List<string>? Sources { get; set; }
    }

    /// <summary>
    /// نموذج طلب إنشاء محادثة جديدة
    /// </summary>
    public class CreateConversationRequestDTO
    {
        /// <summary>
        /// عنوان المحادثة
        /// </summary>
        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        /// <summary>
        /// الاستعلام المبدئي
        /// </summary>
        [JsonPropertyName("initialQuery")]
        public string? InitialQuery { get; set; }
    }

    /// <summary>
    /// نموذج ملخص المحادثة
    /// </summary>
    public class ConversationSummaryDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; } = string.Empty;

        /// <summary>
        /// موضوع المحادثة
        /// </summary>
        [JsonPropertyName("topic")]
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// آخر استعلام
        /// </summary>
        [JsonPropertyName("lastQuery")]
        public string LastQuery { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ آخر رسالة
        /// </summary>
        [JsonPropertyName("lastMessageDate")]
        public DateTime LastMessageDate { get; set; }
    }

    /// <summary>
    /// نموذج رسالة محادثة
    /// </summary>
    public class ConversationMessageDTO
    {
        /// <summary>
        /// استعلام المستخدم
        /// </summary>
        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// رد النظام
        /// </summary>
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ الرسالة
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// نموذج تفاصيل المحادثة
    /// </summary>
    public class ConversationDetailDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; } = string.Empty;

        /// <summary>
        /// موضوع المحادثة
        /// </summary>
        [JsonPropertyName("topic")]
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// رسائل المحادثة
        /// </summary>
        [JsonPropertyName("messages")]
        public List<ConversationMessageDTO> Messages { get; set; } = new List<ConversationMessageDTO>();

        /// <summary>
        /// إجمالي عدد الرسائل
        /// </summary>
        [JsonPropertyName("totalMessages")]
        public int TotalMessages { get; set; }

        /// <summary>
        /// رقم الصفحة الحالية
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// إجمالي عدد الصفحات
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
} 