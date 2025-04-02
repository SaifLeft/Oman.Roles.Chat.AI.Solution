using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// نموذج طلب التحقق من صلاحية الكوبون
    /// Model for coupon validation request
    /// </summary>
    public class ValidateCouponRequest
    {
        /// <summary>
        /// كود الكوبون
        /// Coupon code
        /// </summary>
        [JsonPropertyName("couponCode")]
        public string CouponCode { get; set; } = string.Empty;

        /// <summary>
        /// معرف خطة الاشتراك
        /// Subscription plan ID
        /// </summary>
        [JsonPropertyName("planId")]
        public string PlanId { get; set; } = string.Empty;
    }
}