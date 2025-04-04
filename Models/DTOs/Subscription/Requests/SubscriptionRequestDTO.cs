using Models.DTOs.Subscription.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// DTO for subscription request
    /// </summary>
    public class SubscriptionRequestDTO
    {
        /// <summary>
        /// Plan ID
        /// </summary>
        [Required]
        public string PlanId { get; set; }

        /// <summary>
        /// Period type (monthly or yearly)
        /// </summary>
        [Required]
        public SubscriptionPeriodType PeriodType { get; set; }

        /// <summary>
        /// Optional coupon code
        /// </summary>
        public string CouponCode { get; set; }
    }
} 