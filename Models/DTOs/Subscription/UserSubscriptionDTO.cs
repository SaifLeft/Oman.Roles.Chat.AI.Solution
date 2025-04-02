using Models.DTOs.Subscription.Enums;
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
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// معرف المستخدم
        /// User ID
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// خطة الاشتراك
        /// Subscription plan
        /// </summary>
        [JsonPropertyName("plan")]
        public SubscriptionPlanDTO Plan { get; set; } = new SubscriptionPlanDTO();

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
        /// هل الاشتراك نشط
        /// Is the subscription active
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// نوع الفترة (شهري/سنوي)
        /// Period type (monthly/yearly)
        /// </summary>
        [JsonPropertyName("periodType")]
        public SubscriptionPeriodType PeriodType { get; set; }

        /// <summary>
        /// تجديد تلقائي
        /// Auto renewal
        /// </summary>
        [JsonPropertyName("autoRenew")]
        public bool AutoRenew { get; set; }

        /// <summary>
        /// كوبون الخصم المستخدم (إن وجد)
        /// Used discount coupon (if any)
        /// </summary>
        [JsonPropertyName("discountCoupon")]
        public DiscountCouponDTO? DiscountCoupon { get; set; }


        public long PlanId { get; set; }

        /// <summary>
        /// Active, Trial, Expired, Cancelled, Suspended
        /// </summary>
        public string Status { get; set; }


    }
}