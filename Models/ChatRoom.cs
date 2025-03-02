using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// نموذج غرفة الدردشة
    /// </summary>
    public class ChatRoomDTO
    {
        /// <summary>
        /// معرف الغرفة
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// عنوان الغرفة
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// وصف الغرفة
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// معرف المستخدم المنشئ
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// قائمة ملفات PDF المرتبطة بالغرفة
        /// </summary>
        [JsonPropertyName("pdfFiles")]
        public List<string> PdfFiles { get; set; } = new();

        /// <summary>
        /// محادثات الغرفة
        /// </summary>
        [JsonPropertyName("messages")]
        public List<ChatMessageDTO> Messages { get; set; } = new();

        /// <summary>
        /// قواعد الدردشة
        /// </summary>
        [JsonPropertyName("rules")]
        public string Rules { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج رسالة الدردشة
    /// </summary>
    public class ChatMessageDTO
    {
        /// <summary>
        /// معرف الرسالة
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// نوع المرسل (مستخدم أو نظام)
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// محتوى الرسالة
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ الإرسال
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// نموذج إنشاء غرفة دردشة جديدة
    /// </summary>
    public class CreateChatRoomRequest
    {
        /// <summary>
        /// عنوان الغرفة
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// وصف الغرفة
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// قائمة ملفات PDF المرتبطة بالغرفة
        /// </summary>
        [JsonPropertyName("pdfFiles")]
        public List<string> PdfFiles { get; set; } = new();

        /// <summary>
        /// قواعد الدردشة المخصصة (اختياري)
        /// </summary>
        [JsonPropertyName("rules")]
        public string? Rules { get; set; }
    }

    /// <summary>
    /// نموذج طلب إرسال رسالة جديدة
    /// </summary>
    public class SendMessageRequest
    {
        /// <summary>
        /// معرف غرفة الدردشة
        /// </summary>
        [JsonPropertyName("roomId")]
        public string RoomId { get; set; } = string.Empty;

        /// <summary>
        /// محتوى الرسالة
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج استجابة الرسالة
    /// </summary>
    public class ChatResponseModel
    {
        /// <summary>
        /// معرف غرفة الدردشة
        /// </summary>
        [JsonPropertyName("roomId")]
        public string RoomId { get; set; } = string.Empty;

        /// <summary>
        /// رسالة المستخدم
        /// </summary>
        [JsonPropertyName("userMessage")]
        public ChatMessageDTO UserMessage { get; set; } = new();

        /// <summary>
        /// رسالة النظام (Deep Seek)
        /// </summary>
        [JsonPropertyName("systemResponse")]
        public ChatMessageDTO SystemResponse { get; set; } = new();
    }
}