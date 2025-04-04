using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription
{
    /// <summary>
    /// نموذج كوبون الخصم
    /// Discount coupon model
    /// </summary>
    public class DiscountCouponDTO
    {
        /// <summary>
        /// معرف الكوبون
        /// Coupon ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// كود الكوبون
        /// Coupon code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// قيمة الخصم
        /// Discount value
        /// </summary>
        [JsonPropertyName("discountValue")]
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// نوع الخصم (نسبة مئوية أو قيمة ثابتة)
        /// Discount type (percentage or fixed amount)
        /// </summary>
        [JsonPropertyName("discountType")]
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// تاريخ انتهاء الصلاحية
        /// Expiry date
        /// </summary>
        [JsonPropertyName("expiryDate")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// هل الكوبون نشط
        /// Is the coupon active
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// علامات الكوبون
        /// Coupon tags
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// وصف الكوبون
        /// Coupon description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

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
        public List<string> ApplicablePlanIds { get; set; } = new List<string>();
        
        /// <summary>
        /// تاريخ نهاية الصلاحية
        /// End date
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// عدد مرات استخدام الكوبون الحالي
        /// Current usage count
        /// </summary>
        [JsonPropertyName("currentUses")]
        public int CurrentUses { get; set; }
        
        /// <summary>
        /// خطط الاشتراك التي يمكن تطبيق الكوبون عليها
        /// Subscription plans the coupon can be applied to
        /// </summary>
        [JsonPropertyName("applicablePlans")]
        public List<PlanReferenceDTO> ApplicablePlans { get; set; } = new List<PlanReferenceDTO>();
    }
    
    /// <summary>
    /// مرجع خطة اشتراك مختصر
    /// Brief subscription plan reference
    /// </summary>
    public class PlanReferenceDTO
    {
        /// <summary>
        /// معرف الخطة
        /// Plan ID
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        /// <summary>
        /// اسم الخطة
        /// Plan name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}