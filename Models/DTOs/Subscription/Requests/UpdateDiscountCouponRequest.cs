using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// نموذج طلب تحديث كوبون خصم
    /// Model for updating a discount coupon
    /// </summary>
    public class UpdateDiscountCouponRequestDTO
    {
        /// <summary>
        /// قيمة الخصم
        /// Discount value
        /// </summary>
        [JsonPropertyName("discountValue")]
        public decimal? DiscountValue { get; set; }

        /// <summary>
        /// نوع الخصم (نسبة مئوية أو قيمة ثابتة)
        /// Discount type (percentage or fixed amount)
        /// </summary>
        [JsonPropertyName("discountType")]
        public DiscountType? DiscountType { get; set; }

        /// <summary>
        /// تاريخ انتهاء الصلاحية
        /// Expiry date
        /// </summary>
        [JsonPropertyName("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// هل الكوبون نشط
        /// Is the coupon active
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// علامات الكوبون
        /// Coupon tags
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        /// <summary>
        /// وصف الكوبون
        /// Coupon description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// تاريخ بدء الصلاحية
        /// Start date
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// الحد الأقصى لعدد مرات الاستخدام
        /// Maximum number of uses
        /// </summary>
        [JsonPropertyName("maxUses")]
        public int? MaxUses { get; set; }

        /// <summary>
        /// معرفات خطط الاشتراك التي يمكن تطبيق الكوبون عليها
        /// IDs of subscription plans the coupon can be applied to
        /// </summary>
        [JsonPropertyName("applicablePlanIds")]
        public List<string>? ApplicablePlanIds { get; set; }
        
        /// <summary>
        /// كود الكوبون
        /// Coupon code
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }
        
        /// <summary>
        /// تاريخ نهاية الصلاحية
        /// End date
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// معرفات خطط الاشتراك
        /// Plan IDs
        /// </summary>
        [JsonPropertyName("planIds")]
        public List<string>? PlanIds { get; set; }
    }
}