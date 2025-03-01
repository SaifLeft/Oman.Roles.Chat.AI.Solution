using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// نموذج ملف PDF
    /// </summary>
    public class PdfFile
    {
        /// <summary>
        /// اسم الملف
        /// </summary>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// العنوان الوصفي للملف
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// وصف الملف
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ الرفع
        /// </summary>
        [JsonPropertyName("uploadDate")]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرف المستخدم الذي قام برفع الملف
        /// </summary>
        [JsonPropertyName("uploadedBy")]
        public string UploadedBy { get; set; } = string.Empty;

        /// <summary>
        /// حجم الملف بالبايت
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>
        /// الكلمات المفتاحية للملف
        /// </summary>
        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = new();
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
        public List<PdfFile> Files { get; set; } = new();

        /// <summary>
        /// إجمالي عدد الملفات
        /// </summary>
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}