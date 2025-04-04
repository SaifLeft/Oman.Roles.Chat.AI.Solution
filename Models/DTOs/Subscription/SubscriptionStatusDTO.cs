using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription
{
    /// <summary>
    /// يمثل حالة اشتراك المستخدم
    /// </summary>
    public class SubscriptionStatusDTO
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        /// <summary>
        /// اسم خطة الاشتراك
        /// </summary>
        [JsonPropertyName("planName")]
        public string PlanName { get; set; } = string.Empty;

        /// <summary>
        /// هل الاشتراك فعال؟
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// تاريخ بدء الاشتراك
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ انتهاء الاشتراك
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// عدد الأيام المتبقية في الاشتراك
        /// </summary>
        [JsonPropertyName("daysRemaining")]
        public long DaysRemaining { get; set; }

        /// <summary>
        /// عدد الاستعلامات المسموح بها شهرياً
        /// </summary>
        [JsonPropertyName("monthlyQueryLimit")]
        public long MonthlyQueryLimit { get; set; }

        /// <summary>
        /// عدد الاستعلامات المستخدمة هذا الشهر
        /// </summary>
        [JsonPropertyName("queriesUsedThisMonth")]
        public long QueriesUsedThisMonth { get; set; }

        /// <summary>
        /// عدد الاستعلامات المتبقية هذا الشهر
        /// </summary>
        [JsonPropertyName("remainingQueries")]
        public long RemainingQueries => MonthlyQueryLimit - QueriesUsedThisMonth;

        /// <summary>
        /// نسبة الاستعلامات المستخدمة (0-100)
        /// </summary>
        [JsonPropertyName("usagePercentage")]
        public long UsagePercentage => MonthlyQueryLimit > 0 ? (QueriesUsedThisMonth * 100 / MonthlyQueryLimit) : 0;

        /// <summary>
        /// هل الاشتراك تجريبي؟
        /// </summary>
        [JsonPropertyName("isTrial")]
        public bool IsTrial { get; set; }

        /// <summary>
        /// هل تم تجاوز الحد الشهري للاستعلامات؟
        /// </summary>
        [JsonPropertyName("isLimitExceeded")]
        public bool IsLimitExceeded => QueriesUsedThisMonth >= MonthlyQueryLimit;

        /// <summary>
        /// حجم الملفات المسموح برفعها (بالميجابايت)
        /// </summary>
        [JsonPropertyName("fileUploadSizeLimitMB")]
        public long FileUploadSizeLimitMB { get; set; }

        /// <summary>
        /// حالة الاشتراك النصية
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        public List<string> Features { get; set; }
    }

    /// <summary>
    /// نموذج استخدام الاستعلامات
    /// </summary>
    public class QueryUsageDTO
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        /// <summary>
        /// هل مشترك في الخطة المدفوعة
        /// </summary>
        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        /// <summary>
        /// عدد الاستعلامات المستخدمة
        /// </summary>
        [JsonPropertyName("queryUsage")]
        public long QueryUsage { get; set; }

        /// <summary>
        /// الحد الأقصى للاستعلامات
        /// </summary>
        [JsonPropertyName("queryLimit")]
        public long QueryLimit { get; set; }

        /// <summary>
        /// عدد الاستعلامات المتبقية
        /// </summary>
        [JsonPropertyName("remainingQueries")]
        public long RemainingQueries { get; set; }
    }
}