using System.Text.Json.Serialization;

namespace Models
{
    // These models are continued from the previous file

    /// <summary>
    /// نموذج طلب إنشاء اشتراك جديد
    /// </summary>
    public class CreateSubscriptionRequest
    {
        /// <summary>
        /// معرف خطة الاشتراك
        /// </summary>
        [JsonPropertyName("planId")]
        public string PlanId { get; set; } = string.Empty;

        /// <summary>
        /// نوع الفترة (شهري/سنوي)
        /// </summary>
        [JsonPropertyName("periodType")]
        public SubscriptionPeriodType PeriodType { get; set; } = SubscriptionPeriodType.Monthly;

        /// <summary>
        /// كود الكوبون (اختياري)
        /// </summary>
        [JsonPropertyName("couponCode")]
        public string? CouponCode { get; set; }

        /// <summary>
        /// تجديد تلقائي
        /// </summary>
        [JsonPropertyName("autoRenew")]
        public bool AutoRenew { get; set; } = true;
    }

    /// <summary>
    /// نموذج طلب تجديد الاشتراك
    /// </summary>
    public class RenewSubscriptionRequest
    {
        /// <summary>
        /// معرف الاشتراك
        /// </summary>
        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; } = string.Empty;

        /// <summary>
        /// كود الكوبون (اختياري)
        /// </summary>
        [JsonPropertyName("couponCode")]
        public string? CouponCode { get; set; }
    }

    /// <summary>
    /// نموذج طلب التحقق من صلاحية كوبون
    /// </summary>
    public class ValidateCouponRequest
    {
        /// <summary>
        /// كود الكوبون
        /// </summary>
        [JsonPropertyName("couponCode")]
        public string CouponCode { get; set; } = string.Empty;

        /// <summary>
        /// معرف خطة الاشتراك
        /// </summary>
        [JsonPropertyName("planId")]
        public string PlanId { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج استجابة التحقق من صلاحية كوبون
    /// </summary>
    public class CouponValidationResponse
    {
        /// <summary>
        /// صالح
        /// </summary>
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// قيمة الخصم
        /// </summary>
        [JsonPropertyName("discountValue")]
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// نوع الخصم
        /// </summary>
        [JsonPropertyName("discountType")]
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// رسالة الخطأ (في حالة عدم الصلاحية)
        /// </summary>
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }


    /// <summary>
    /// نموذج معاملة مالية
    /// </summary>
    public class FinancialTransaction
    {
        /// <summary>
        /// معرف المعاملة
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// معرف المستخدم
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// معرف الاشتراك
        /// </summary>
        [JsonPropertyName("subscriptionId")]
        public string? SubscriptionId { get; set; }

        /// <summary>
        /// معرف خطة الاشتراك
        /// </summary>
        [JsonPropertyName("planId")]
        public string PlanId { get; set; } = string.Empty;

        /// <summary>
        /// نوع المعاملة
        /// </summary>
        [JsonPropertyName("type")]
        public TransactionType Type { get; set; }

        /// <summary>
        /// المبلغ
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// قيمة الخصم
        /// </summary>
        [JsonPropertyName("discountAmount")]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// المبلغ الإجمالي
        /// </summary>
        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// كود الكوبون
        /// </summary>
        [JsonPropertyName("couponCode")]
        public string? CouponCode { get; set; }

        /// <summary>
        /// رقم الفاتورة
        /// </summary>
        [JsonPropertyName("invoiceNumber")]
        public string InvoiceNumber { get; set; } = string.Empty;

        /// <summary>
        /// حالة المعاملة
        /// </summary>
        [JsonPropertyName("status")]
        public TransactionStatus Status { get; set; } = TransactionStatus.Completed;

        /// <summary>
        /// معرف المعاملة لدى بوابة الدفع
        /// </summary>
        [JsonPropertyName("paymentGatewayTransactionId")]
        public string? PaymentGatewayTransactionId { get; set; }

        /// <summary>
        /// وسيلة الدفع
        /// </summary>
        [JsonPropertyName("paymentMethod")]
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// تاريخ المعاملة
        /// </summary>
        [JsonPropertyName("transactionDate")]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ملاحظات
        /// </summary>
        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionPeriodType PeriodType { get; set; }
        public bool AutoRenew { get; set; }
        public string LastInvoiceNumber { get; set; }
    }

    /// <summary>
    /// نوع فترة الاشتراك
    /// </summary>
    public enum SubscriptionPeriodType
    {
        /// <summary>
        /// شهري
        /// </summary>
        Monthly = 0,

        /// <summary>
        /// سنوي
        /// </summary>
        Yearly = 1
    }

    /// <summary>
    /// حالة الاشتراك
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// نشط
        /// </summary>
        Active = 0,

        /// <summary>
        /// تجريبي
        /// </summary>
        Trial = 1,

        /// <summary>
        /// منتهي
        /// </summary>
        Expired = 2,

        /// <summary>
        /// ملغى
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// معلق
        /// </summary>
        Suspended = 4
    }

    /// <summary>
    /// نوع الخصم
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// نسبة مئوية
        /// </summary>
        Percentage = 0,

        /// <summary>
        /// قيمة ثابتة
        /// </summary>
        Fixed = 1
    }

    /// <summary>
    /// نوع المعاملة
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// اشتراك جديد
        /// </summary>
        NewSubscription = 0,

        /// <summary>
        /// تجديد
        /// </summary>
        Renewal = 1,

        /// <summary>
        /// ترقية
        /// </summary>
        Upgrade = 2,

        /// <summary>
        /// تخفيض
        /// </summary>
        Downgrade = 3,

        /// <summary>
        /// استرداد
        /// </summary>
        Refund = 4
    }

    /// <summary>
    /// حالة المعاملة
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// معلقة
        /// </summary>
        Pending = 0,

        /// <summary>
        /// مكتملة
        /// </summary>
        Completed = 1,

        /// <summary>
        /// فاشلة
        /// </summary>
        Failed = 2,

        /// <summary>
        /// ملغاة
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// مستردة
        /// </summary>
        Refunded = 4
    }
}