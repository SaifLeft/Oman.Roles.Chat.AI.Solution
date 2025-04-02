using Models.DTOs.Subscription.Enums;
using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// نموذج طلب الاشتراك في خطة
    /// Model for subscription request
    /// </summary>
    public class SubscribeRequest
    {
        /// <summary>
        /// معرف خطة الاشتراك
        /// Subscription plan ID
        /// </summary>
        [JsonPropertyName("planId")]
        public string PlanId { get; set; } = string.Empty;

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