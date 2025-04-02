using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Responses
{
    /// <summary>
    /// نموذج استجابة التحقق من صلاحية الكوبون
    /// Model for coupon validation response
    /// </summary>
    public class CouponValidationResponse
    {
        /// <summary>
        /// هل الكوبون صالح
        /// Is the coupon valid
        /// </summary>
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// قيمة الخصم
        /// Discount value
        /// </summary>
        [JsonPropertyName("discountValue")]
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// نوع الخصم
        /// Discount type
        /// </summary>
        [JsonPropertyName("discountType")]
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// السعر بعد الخصم
        /// Price after discount
        /// </summary>
        [JsonPropertyName("priceAfterDiscount")]
        public decimal PriceAfterDiscount { get; set; }
    }
}