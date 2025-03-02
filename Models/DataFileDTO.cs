using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// نموذج ملف PDF
    /// </summary>
    /// <summary>
    /// نموذج ملف PDF
    /// </summary>
    public class DataFileDTO
    {
        /// <summary>
        /// معرف الملف
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// اسم الملف
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// عنوان الملف
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// وصف الملف
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// مسار الملف على الخادم
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// حجم الملف بالبايت
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// حجم الملف منسق للعرض (مثال: 2.5 MB)
        /// </summary>
        public string SizeFormatted { get; set; } = string.Empty;

        /// <summary>
        /// نوع المحتوى
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// الكلمات المفتاحية المرتبطة بالملف
        /// </summary>
        public List<string> Keywords { get; set; } = new List<string>();

        /// <summary>
        /// معرف المستخدم الذي أنشأ الملف
        /// </summary>
        public string CreatedByUserId { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ إنشاء الملف
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// معرف المستخدم الذي عدل الملف
        /// </summary>
        public string? ModifiedByUserId { get; set; }

        /// <summary>
        /// تاريخ تعديل الملف
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// رابط تنزيل الملف
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; }
    }

    /// <summary>
    /// نموذج استجابة قائمة ملفات PDF
    /// </summary>
    public class PdfFilesResponse
    {
        /// <summary>
        /// قائمة الملفات
        /// </summary>
        [JsonPropertyName("files")]
        public List<DataFileDTO> Files { get; set; } = new();

        /// <summary>
        /// إجمالي عدد الملفات
        /// </summary>
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}