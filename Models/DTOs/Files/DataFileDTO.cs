using System.Text.Json.Serialization;

namespace Models.DTOs.Files
{
    /// <summary>
    /// نموذج يمثل ملف بيانات
    /// </summary>
    public class DataFileDTO
    {
        /// <summary>
        /// معرف الملف
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// معرف المستخدم الذي رفع الملف
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        /// <summary>
        /// اسم الملف
        /// </summary>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الأصلي للملف
        /// </summary>
        [JsonPropertyName("originalName")]
        public string OriginalName { get; set; } = string.Empty;

        /// <summary>
        /// مسار الملف
        /// </summary>
        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// حجم الملف بالبايت
        /// </summary>
        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// تاريخ رفع الملف
        /// </summary>
        [JsonPropertyName("uploadDate")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        /// <summary>
        /// هل الملف نشط
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// عدد مرات تنزيل الملف
        /// </summary>
        [JsonPropertyName("downloadCount")]
        public int DownloadCount { get; set; } = 0;

        /// <summary>
        /// المحتوى النصي المستخرج من الملف (إن كان PDF)
        /// </summary>
        [JsonPropertyName("extractedText")]
        public string? ExtractedText { get; set; }

        /// <summary>
        /// عدد صفحات الملف (إن كان PDF)
        /// </summary>
        [JsonPropertyName("pageCount")]
        public int? PageCount { get; set; }
    }
} 