using System.Text.Json.Serialization;
using Models.DTOs.Subscription;
using Models.DTOs.Subscription.Enums;

/// <summary>
/// نموذج معاملة مالية
/// </summary>
public class FinancialTransactionDTO
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
    public DateTime TransactionDate { get; set; } = DateTime.Now;

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