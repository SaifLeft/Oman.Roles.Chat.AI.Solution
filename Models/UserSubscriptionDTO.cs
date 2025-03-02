namespace Models
{
    public class UserSubscriptionDTO
    {
        public string Id { get; set; }
        public SubscriptionStatus Status { get; set; }
        public string PlanId { get; set; }
        public string UserId { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public SubscriptionPeriodType PeriodType { get; set; }
        public bool AutoRenew { get; set; }
        public string LastInvoiceNumber { get; set; }
        public DateTime LastRenewalDate { get; set; }
        public string? Notes { get; set; }
    }
    public class SubscriptionPlanDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }
        public int AllowedChatRooms { get; set; }
        public int AllowedFiles { get; set; }
        public int AllowedFileSizeMb { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrial { get; set; }
        public List<string> Features { get; set; }
        public List<string> Tags { get; set; }
        public int? TrialDays { get; set; }
    }

    //DiscountCoupon
    public class DiscountCouponDTO
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUses { get; set; }
        public int? CurrentUses { get; set; }
        public List<string> ApplicablePlanIds { get; set; }
    }
}
