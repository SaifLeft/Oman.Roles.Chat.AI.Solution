using FluentValidation;
using System.Text.Json.Serialization;

namespace Models.DTOs.AIChat
{
    /// <summary>
    /// يمثل ملخص محادثة
    /// </summary>
    public class AIChatConversationSummaryDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// عنوان المحادثة
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ إنشاء المحادثة
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// تاريخ آخر تحديث للمحادثة
        /// </summary>
        [JsonPropertyName("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        /// عدد الرسائل في المحادثة
        /// </summary>
        [JsonPropertyName("messageCount")]
        public int MessageCount { get; set; }

        /// <summary>
        /// آخر رسالة في المحادثة
        /// </summary>
        [JsonPropertyName("lastMessage")]
        public string LastMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// يمثل تفاصيل محادثة كاملة
    /// </summary>
    public class AIChatConversationDetailDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// عنوان المحادثة
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ إنشاء المحادثة
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// تاريخ آخر تحديث للمحادثة
        /// </summary>
        [JsonPropertyName("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        /// قائمة الرسائل في المحادثة
        /// </summary>
        [JsonPropertyName("messages")]
        public List<AIChatMessageDTO> Messages { get; set; } = new List<AIChatMessageDTO>();

        /// <summary>
        /// معلومات الترقيم للرسائل
        /// </summary>
        [JsonPropertyName("pagination")]
        public PaginationInfoDTO Pagination { get; set; } = new PaginationInfoDTO();
    }

    /// <summary>
    /// يمثل رسالة في محادثة
    /// </summary>
    public class AIChatMessageDTO
    {
        /// <summary>
        /// معرف الرسالة
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// نص الرسالة
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// نوع الرسالة (مستخدم أو نظام)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ إنشاء الرسالة
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// قائمة المستندات المرجعية المرتبطة بالرسالة
        /// </summary>
        [JsonPropertyName("documentReferences")]
        public List<DocumentReferenceDTO>? DocumentReferences { get; set; }

        /// <summary>
        /// قائمة الملفات المرفقة بالرسالة
        /// </summary>
        [JsonPropertyName("attachments")]
        public List<AttachmentDTO>? Attachments { get; set; }
    }

    /// <summary>
    /// يمثل مرفق ملف
    /// </summary>
    public class AttachmentDTO
    {
        /// <summary>
        /// معرف المرفق
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// اسم الملف
        /// </summary>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// نوع الملف
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// حجم الملف بالبايت
        /// </summary>
        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// رابط للوصول إلى الملف
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    /// <summary>
    /// معلومات الترقيم
    /// </summary>
    public class PaginationInfoDTO
    {
        /// <summary>
        /// رقم الصفحة الحالية
        /// </summary>
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// إجمالي عدد العناصر
        /// </summary>
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// إجمالي عدد الصفحات
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// هل توجد صفحة سابقة؟
        /// </summary>
        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// هل توجد صفحة تالية؟
        /// </summary>
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => PageNumber < TotalPages;
    }

    /// <summary>
    /// طلب إنشاء محادثة جديدة
    /// </summary>
    public class CreateConversationRequestDTO
    {
        /// <summary>
        /// عنوان المحادثة
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// الرسالة الأولى في المحادثة (اختياري)
        /// </summary>
        [JsonPropertyName("initialMessage")]
        public string? InitialMessage { get; set; }

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
    /// مدقق صحة طلب إنشاء محادثة
    /// </summary>
    public class CreateConversationRequestDTOValidator : AbstractValidator<CreateConversationRequestDTO>
    {
        public CreateConversationRequestDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("يجب إدخال عنوان للمحادثة")
                .MaximumLength(100).WithMessage("يجب ألا يتجاوز عنوان المحادثة 100 حرف");

            When(x => !string.IsNullOrEmpty(x.InitialMessage), () =>
            {
                RuleFor(x => x.InitialMessage)
                    .MaximumLength(4000).WithMessage("يجب ألا يتجاوز طول الرسالة الأولية 4000 حرف");
            });
        }
    }
} 