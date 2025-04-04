using System.Text.Json.Serialization;

namespace Models.Common
{
    /// <summary>
    /// نموذج فترة التحليلات
    /// </summary>
    public class AnalyticsPeriodQuery
    {
        /// <summary>
        /// تاريخ البداية للتحليلات
        /// </summary>
        [JsonPropertyName("fromDate")]
        public DateTime FromDate { get; set; }
        
        /// <summary>
        /// تاريخ النهاية للتحليلات
        /// </summary>
        [JsonPropertyName("toDate")]
        public DateTime ToDate { get; set; }
        
        /// <summary>
        /// لغة العرض (ar أو en)
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; } = "ar";
    }
} 