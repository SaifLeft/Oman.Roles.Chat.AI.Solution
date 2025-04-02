using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// نموذج طلب تحديث خطة اشتراك
    /// Model for updating a subscription plan
    /// </summary>
    public class UpdateSubscriptionPlanRequestDTO
    {
        /// <summary>
        /// اسم خطة الاشتراك
        /// Subscription plan name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// وصف خطة الاشتراك
        /// Subscription plan description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// السعر الشهري
        /// Monthly price
        /// </summary>
        [JsonPropertyName("priceMonthly")]
        public decimal? PriceMonthly { get; set; }

        /// <summary>
        /// السعر السنوي
        /// Yearly price
        /// </summary>
        [JsonPropertyName("priceYearly")]
        public decimal? PriceYearly { get; set; }

        /// <summary>
        /// عدد غرف الدردشة المسموح بها
        /// Number of allowed chat rooms
        /// </summary>
        [JsonPropertyName("allowedChatRooms")]
        public int? AllowedChatRooms { get; set; }

        /// <summary>
        /// عدد الملفات المسموح بها
        /// Number of allowed files
        /// </summary>
        [JsonPropertyName("allowedFiles")]
        public int? AllowedFiles { get; set; }

        /// <summary>
        /// الحجم المسموح به للملفات بالميجابايت
        /// Allowed file size in MB
        /// </summary>
        [JsonPropertyName("allowedFileSizeMb")]
        public int? AllowedFileSizeMb { get; set; }

        /// <summary>
        /// هل الخطة نشطة
        /// Is the plan active
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// هل الخطة تجريبية
        /// Is the plan a trial plan
        /// </summary>
        [JsonPropertyName("isTrial")]
        public bool? IsTrial { get; set; }

        /// <summary>
        /// مميزات الخطة
        /// Plan features
        /// </summary>
        [JsonPropertyName("features")]
        public List<string>? Features { get; set; }

        /// <summary>
        /// علامات الخطة
        /// Plan tags
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        /// <summary>
        /// عدد أيام الفترة التجريبية
        /// Number of trial days
        /// </summary>
        [JsonPropertyName("trialDays")]
        public int? TrialDays { get; set; }
    }
}