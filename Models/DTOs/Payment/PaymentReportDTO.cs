using System;
using System.Collections.Generic;

namespace Models.DTOs.Payment
{
    /// <summary>
    /// نموذج لتقارير المدفوعات
    /// Payment reporting model
    /// </summary>
    public class PaymentReportDTO
    {
        /// <summary>
        /// إجمالي الإيرادات
        /// Total revenue
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// عدد المدفوعات
        /// Number of payments
        /// </summary>
        public int TotalPayments { get; set; }

        /// <summary>
        /// عدد المشتركين النشطين
        /// Number of active subscribers
        /// </summary>
        public int ActiveSubscribers { get; set; }

        /// <summary>
        /// متوسط قيمة الاشتراك
        /// Average subscription value
        /// </summary>
        public decimal AverageSubscriptionValue { get; set; }

        /// <summary>
        /// نسبة النمو مقارنة بالفترة السابقة
        /// Growth percentage compared to previous period
        /// </summary>
        public decimal GrowthPercentage { get; set; }

        /// <summary>
        /// تاريخ بداية الفترة
        /// Period start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ نهاية الفترة
        /// Period end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// توزيع المدفوعات حسب طريقة الدفع
        /// Payment distribution by payment method
        /// </summary>
        public Dictionary<string, int> PaymentMethodDistribution { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// توزيع المدفوعات حسب خطة الاشتراك
        /// Payment distribution by subscription plan
        /// </summary>
        public Dictionary<string, int> SubscriptionPlanDistribution { get; set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// إجمالي الإيرادات في الأشهر الماضية
        /// Total revenue by month for past months
        /// </summary>
        public List<MonthlyRevenueDTO> MonthlyRevenue { get; set; } = new List<MonthlyRevenueDTO>();
    }

    /// <summary>
    /// نموذج الإيرادات الشهرية
    /// Monthly revenue model
    /// </summary>
    public class MonthlyRevenueDTO
    {
        /// <summary>
        /// الشهر (1-12)
        /// Month (1-12)
        /// </summary>
        public int Month { get; set; }
        
        /// <summary>
        /// السنة
        /// Year
        /// </summary>
        public int Year { get; set; }
        
        /// <summary>
        /// إجمالي الإيرادات
        /// Total revenue
        /// </summary>
        public decimal Revenue { get; set; }
        
        /// <summary>
        /// عدد المدفوعات
        /// Number of payments
        /// </summary>
        public int PaymentCount { get; set; }
    }
} 