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
        public long PlanId { get; set; }

        /// <summary>
        /// هل الاشتراك سنوي؟
        /// Is the subscription yearly?
        /// </summary>
        [JsonPropertyName("isYearly")]
        public bool IsYearly { get; set; }

        /// <summary>
        /// رمز الخصم (إن وجد)
        /// Coupon code (if any)
        /// </summary>
        [JsonPropertyName("couponCode")]
        public string? CouponCode { get; set; }

        /// <summary>
        /// عنوان IP للمستخدم
        /// User's IP address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string? IpAddress { get; set; }

        /// <summary>
        /// معلومات متصفح المستخدم
        /// User's browser information
        /// </summary>
        [JsonPropertyName("userAgent")]
        public string? UserAgent { get; set; }
        
        /// <summary>
        /// نوع فترة الاشتراك (شهري/سنوي)
        /// Subscription period type (monthly/yearly)
        /// </summary>
        [JsonPropertyName("periodType")]
        public string PeriodType { get; set; } = "Monthly";
        
        /// <summary>
        /// هل يتم التجديد تلقائياً؟
        /// Auto-renew?
        /// </summary>
        [JsonPropertyName("autoRenew")]
        public bool? AutoRenew { get; set; }
        
        /// <summary>
        /// معرف المستخدم
        /// User ID
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        
        /// <summary>
        /// معرف عملية الدفع من بوابة الدفع
        /// Payment gateway transaction ID
        /// </summary>
        [JsonPropertyName("paymentGatewayTransactionId")]
        public string? PaymentGatewayTransactionId { get; set; }
        
        /// <summary>
        /// طريقة الدفع المستخدمة
        /// Payment method used
        /// </summary>
        [JsonPropertyName("paymentMethod")]
        public string? PaymentMethod { get; set; }
    }
}
