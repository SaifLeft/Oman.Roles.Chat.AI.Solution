using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription
{
    /// <summary>
    /// نموذج تقرير الاشتراكات
    /// Subscription report model
    /// </summary>
    public class SubscriptionReportDTO
    {
        /// <summary>
        /// العدد الإجمالي للاشتراكات
        /// Total subscriptions
        /// </summary>
        [JsonPropertyName("totalSubscriptions")]
        public int TotalSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات النشطة
        /// Active subscriptions
        /// </summary>
        [JsonPropertyName("activeSubscriptions")]
        public int ActiveSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات المنتهية
        /// Expired subscriptions
        /// </summary>
        [JsonPropertyName("expiredSubscriptions")]
        public int ExpiredSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات الملغاة
        /// Cancelled subscriptions
        /// </summary>
        [JsonPropertyName("cancelledSubscriptions")]
        public int CancelledSubscriptions { get; set; }

        /// <summary>
        /// عدد الاشتراكات التجريبية
        /// Trial subscriptions
        /// </summary>
        [JsonPropertyName("trialSubscriptions")]
        public int TrialSubscriptions { get; set; }
        
        /// <summary>
        /// تاريخ بداية التقرير
        /// Report start date
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// تاريخ نهاية التقرير
        /// Report end date
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// إجمالي الاشتراكات النشطة
        /// Total active subscriptions
        /// </summary>
        [JsonPropertyName("totalActiveSubscriptions")]
        public int TotalActiveSubscriptions { get; set; }
        
        /// <summary>
        /// الاشتراكات الجديدة خلال الفترة
        /// New subscriptions during period
        /// </summary>
        [JsonPropertyName("newSubscriptions")]
        public int NewSubscriptions { get; set; }
        
        /// <summary>
        /// إجمالي الإيرادات
        /// Total revenue
        /// </summary>
        [JsonPropertyName("totalRevenue")]
        public decimal TotalRevenue { get; set; }
        
        /// <summary>
        /// توزيع الاشتراكات حسب الخطة
        /// Subscriptions by plan
        /// </summary>
        [JsonPropertyName("subscriptionsByPlan")]
        public List<PlanSubscriptionCountDTO> SubscriptionsByPlan { get; set; } = new List<PlanSubscriptionCountDTO>();
    }
    
    /// <summary>
    /// نموذج عدد الاشتراكات لكل خطة
    /// Subscription count per plan model
    /// </summary>
    public class PlanSubscriptionCountDTO
    {
        /// <summary>
        /// معرف الخطة
        /// Plan ID
        /// </summary>
        [JsonPropertyName("planId")]
        public long PlanId { get; set; }
        
        /// <summary>
        /// اسم الخطة
        /// Plan name
        /// </summary>
        [JsonPropertyName("planName")]
        public string PlanName { get; set; } = string.Empty;
        
        /// <summary>
        /// عدد الاشتراكات
        /// Subscription count
        /// </summary>
        [JsonPropertyName("subscriptionCount")]
        public int SubscriptionCount { get; set; }
    }
}