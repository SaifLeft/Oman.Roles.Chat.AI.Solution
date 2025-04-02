using Models.DTOs.Subscription.Enums;
using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// نموذج طلب تجديد الاشتراك
    /// Model for subscription renewal request
    /// </summary>
    public class RenewSubscriptionRequest
    {
        /// <summary>
        /// معرف الاشتراك
        /// Subscription ID
        /// </summary>
        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; } = string.Empty;

        /// <summary>
        /// نوع الفترة (شهري/سنوي)
        /// Period type (monthly/yearly)
        /// </summary>
        [JsonPropertyName("periodType")]
        public SubscriptionPeriodType PeriodType { get; set; } = SubscriptionPeriodType.Monthly;

        /// <summary>
        /// كود الكوبون (اختياري)
        /// Coupon code (optional)
        /// </summary>
        [JsonPropertyName("couponCode")]
        public string? CouponCode { get; set; }

        /// <summary>
        /// تجديد تلقائي
        /// Auto renewal
        /// </summary>
        [JsonPropertyName("autoRenew")]
        public bool AutoRenew { get; set; } = true;
    }
}