using System;
using System.Collections.Generic;

namespace Models.DTOs.AIChat
{
    /// <summary>
    /// نموذج استجابة الذكاء الاصطناعي
    /// </summary>
    public class AIResponseDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// رد الذكاء الاصطناعي
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// هل تمت العملية بنجاح
        /// </summary>
        public bool IsSuccess { get; set; }
    }

    /// <summary>
    /// نموذج ملخص المحادثة
    /// </summary>
    public class ConversationSummaryDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// موضوع المحادثة
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// آخر استعلام
        /// </summary>
        public string LastQuery { get; set; }

        /// <summary>
        /// تاريخ آخر رسالة
        /// </summary>
        public DateTime LastMessageDate { get; set; }
    }

    /// <summary>
    /// نموذج تفاصيل المحادثة
    /// </summary>
    public class ConversationDetailDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// موضوع المحادثة
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// لغة المحادثة
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// رسائل المحادثة
        /// </summary>
        public List<MessageDTO> Messages { get; set; }
    }

    /// <summary>
    /// نموذج رسالة المحادثة
    /// </summary>
    public class MessageDTO
    {
        /// <summary>
        /// استعلام المستخدم
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// رد الذكاء الاصطناعي
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// تاريخ الرسالة
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
} 