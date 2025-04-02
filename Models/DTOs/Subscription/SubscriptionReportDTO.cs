using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription
{
    /// <summary>
    /// نموذج تقرير الاشتراكات
    /// Subscription report model
    /// </summary>
    public class SubscriptionReportDTO
    {
        /// <summary>
        /// إجمالي عدد الاشتراكات
        /// Total number of subscriptions
        /// </summary>
        [JsonPropertyName("totalSubscriptions")]
        public int TotalSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات النشطة
        /// Number of active subscriptions
        /// </summary>
        [JsonPropertyName("activeSubscriptions")]
        public int ActiveSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات المنتهية
        /// Number of expired subscriptions
        /// </summary>
        [JsonPropertyName("expiredSubscriptions")]
        public int ExpiredSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات الملغاة
        /// Number of cancelled subscriptions
        /// </summary>
        [JsonPropertyName("cancelledSubscriptions")]
        public int CancelledSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات التجريبية
        /// Number of trial subscriptions
        /// </summary>
        [JsonPropertyName("trialSubscriptions")]
        public int TrialSubscriptions { get; set; }
    }
}