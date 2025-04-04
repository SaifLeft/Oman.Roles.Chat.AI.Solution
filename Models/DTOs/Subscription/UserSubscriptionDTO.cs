using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription
{
    /// <summary>
    /// نموذج اشتراك المستخدم
    /// User subscription model
    /// </summary>
    public class UserSubscriptionDTO
    {
        /// <summary>
        /// معرف الاشتراك
        /// Subscription ID
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// معرف المستخدم
        /// User ID
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        /// <summary>
        /// معرف خطة الاشتراك
        /// </summary>
        [JsonPropertyName("planId")]
        public long PlanId { get; set; }

        /// <summary>
        /// اسم خطة الاشتراك
        /// </summary>
        [JsonPropertyName("planName")]
        public string PlanName { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ بدء الاشتراك
        /// Subscription start date
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ انتهاء الاشتراك
        /// Subscription end date
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// حالة الاشتراك
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// عدد الاستعلامات المستخدمة هذا الشهر
        /// </summary>
        [JsonPropertyName("queriesUsedThisMonth")]
        public int QueriesUsedThisMonth { get; set; }

        /// <summary>
        /// عدد الاستعلامات المسموح بها شهرياً
        /// </summary>
        [JsonPropertyName("monthlyQueryLimit")]
        public int MonthlyQueryLimit { get; set; }

        /// <summary>
        /// هل الاشتراك سنوي؟
        /// </summary>
        [JsonPropertyName("isYearly")]
        public bool IsYearly { get; set; }

        /// <summary>
        /// هل تم دفع الاشتراك؟
        /// </summary>
        [JsonPropertyName("isPaid")]
        public bool IsPaid { get; set; }

        /// <summary>
        /// رمز الفاتورة
        /// </summary>
        [JsonPropertyName("invoiceReference")]
        public string? InvoiceReference { get; set; }

        /// <summary>
        /// سعر الاشتراك
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}