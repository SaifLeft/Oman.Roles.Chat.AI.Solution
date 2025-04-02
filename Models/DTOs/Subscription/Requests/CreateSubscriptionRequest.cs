using Models.DTOs.Subscription.Enums;
using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// نموذج طلب إنشاء اشتراك جديد
    /// Model for creating a new subscription
    /// </summary>
    public class CreateSubscriptionRequestDTO
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
        public string PaymentGatewayTransactionId { get; set; }
        public string PaymentMethod { get; set; }
    }
}