using System.Text.Json.Serialization;

namespace Models.DTOs.Admin
{
    /// <summary>
    /// نموذج ملخص لوحة التحكم
    /// </summary>
    public class DashboardSummaryDTO
    {
        /// <summary>
        /// إجمالي عدد المستخدمين
        /// </summary>
        [JsonPropertyName("totalUsers")]
        public int TotalUsers { get; set; }
        
        /// <summary>
        /// المستخدمين الجدد في الفترة المحددة
        /// </summary>
        [JsonPropertyName("newUsers")]
        public int NewUsers { get; set; }
        
        /// <summary>
        /// إجمالي الاشتراكات النشطة
        /// </summary>
        [JsonPropertyName("activeSubscriptions")]
        public int ActiveSubscriptions { get; set; }
        
        /// <summary>
        /// إجمالي الإيرادات في الفترة المحددة
        /// </summary>
        [JsonPropertyName("totalRevenue")]
        public double TotalRevenue { get; set; }
        
        /// <summary>
        /// إجمالي عدد الاستعلامات في الفترة المحددة
        /// </summary>
        [JsonPropertyName("totalQueries")]
        public int TotalQueries { get; set; }
        
        /// <summary>
        /// متوسط الاستعلامات اليومية في الفترة المحددة
        /// </summary>
        [JsonPropertyName("averageDailyQueries")]
        public int AverageDailyQueries { get; set; }
        
        /// <summary>
        /// نسبة النمو في المستخدمين مقارنة بالفترة السابقة
        /// </summary>
        [JsonPropertyName("userGrowthRate")]
        public float UserGrowthRate { get; set; }
        
        /// <summary>
        /// نسبة النمو في الإيرادات مقارنة بالفترة السابقة
        /// </summary>
        [JsonPropertyName("revenueGrowthRate")]
        public float RevenueGrowthRate { get; set; }
    }
    
    /// <summary>
    /// نموذج إحصائيات المستخدمين
    /// </summary>
    public class UserAnalyticsDTO
    {
        /// <summary>
        /// إجمالي عدد المستخدمين
        /// </summary>
        [JsonPropertyName("totalUsers")]
        public int TotalUsers { get; set; }
        
        /// <summary>
        /// المستخدمين الجدد في الفترة المحددة
        /// </summary>
        [JsonPropertyName("newUsers")]
        public int NewUsers { get; set; }
        
        /// <summary>
        /// عدد المستخدمين النشطين في الفترة المحددة
        /// </summary>
        [JsonPropertyName("activeUsers")]
        public int ActiveUsers { get; set; }
        
        /// <summary>
        /// نسبة الاحتفاظ بالمستخدمين
        /// </summary>
        [JsonPropertyName("retentionRate")]
        public float RetentionRate { get; set; }
        
        /// <summary>
        /// بيانات توزيع المستخدمين حسب الخطة
        /// </summary>
        [JsonPropertyName("usersByPlan")]
        public Dictionary<string, int> UsersByPlan { get; set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// بيانات المستخدمين الجدد حسب اليوم
        /// </summary>
        [JsonPropertyName("newUsersByDay")]
        public Dictionary<string, int> NewUsersByDay { get; set; } = new Dictionary<string, int>();
    }
    
    /// <summary>
    /// نموذج إحصائيات الاشتراكات
    /// </summary>
    public class SubscriptionAnalyticsDTO
    {
        /// <summary>
        /// إجمالي عدد الاشتراكات النشطة
        /// </summary>
        [JsonPropertyName("activeSubscriptions")]
        public int ActiveSubscriptions { get; set; }
        
        /// <summary>
        /// اشتراكات جديدة في الفترة المحددة
        /// </summary>
        [JsonPropertyName("newSubscriptions")]
        public int NewSubscriptions { get; set; }
        
        /// <summary>
        /// اشتراكات منتهية في الفترة المحددة
        /// </summary>
        [JsonPropertyName("expiredSubscriptions")]
        public int ExpiredSubscriptions { get; set; }
        
        /// <summary>
        /// معدل تجديد الاشتراكات
        /// </summary>
        [JsonPropertyName("renewalRate")]
        public float RenewalRate { get; set; }
        
        /// <summary>
        /// معدل تحويل المستخدمين المجانيين إلى مدفوعين
        /// </summary>
        [JsonPropertyName("conversionRate")]
        public float ConversionRate { get; set; }
        
        /// <summary>
        /// توزيع الاشتراكات حسب الخطة
        /// </summary>
        [JsonPropertyName("subscriptionsByPlan")]
        public Dictionary<string, int> SubscriptionsByPlan { get; set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// اشتراكات جديدة حسب اليوم
        /// </summary>
        [JsonPropertyName("newSubscriptionsByDay")]
        public Dictionary<string, int> NewSubscriptionsByDay { get; set; } = new Dictionary<string, int>();
    }
    
    /// <summary>
    /// نموذج إحصائيات الاستعلامات
    /// </summary>
    public class QueryAnalyticsDTO
    {
        /// <summary>
        /// إجمالي عدد الاستعلامات في الفترة المحددة
        /// </summary>
        [JsonPropertyName("totalQueries")]
        public int TotalQueries { get; set; }
        
        /// <summary>
        /// متوسط الاستعلامات اليومية
        /// </summary>
        [JsonPropertyName("averageDailyQueries")]
        public float AverageDailyQueries { get; set; }
        
        /// <summary>
        /// متوسط الاستعلامات لكل مستخدم
        /// </summary>
        [JsonPropertyName("averageQueriesPerUser")]
        public float AverageQueriesPerUser { get; set; }
        
        /// <summary>
        /// الاستعلامات حسب اليوم
        /// </summary>
        [JsonPropertyName("queriesByDay")]
        public Dictionary<string, int> QueriesByDay { get; set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// أعلى 10 كلمات مفتاحية في الاستعلامات
        /// </summary>
        [JsonPropertyName("topKeywords")]
        public List<KeywordCountDTO> TopKeywords { get; set; } = new List<KeywordCountDTO>();
        
        /// <summary>
        /// أكثر 10 استعلامات شيوعًا
        /// </summary>
        [JsonPropertyName("topQueries")]
        public List<QueryCountDTO> TopQueries { get; set; } = new List<QueryCountDTO>();
    }
    
    /// <summary>
    /// نموذج إحصائيات المبيعات والإيرادات
    /// </summary>
    public class RevenueAnalyticsDTO
    {
        /// <summary>
        /// إجمالي الإيرادات في الفترة المحددة
        /// </summary>
        [JsonPropertyName("totalRevenue")]
        public decimal TotalRevenue { get; set; }
        
        /// <summary>
        /// متوسط الإيرادات اليومية
        /// </summary>
        [JsonPropertyName("averageDailyRevenue")]
        public decimal AverageDailyRevenue { get; set; }
        
        /// <summary>
        /// إجمالي عدد المعاملات
        /// </summary>
        [JsonPropertyName("totalTransactions")]
        public int TotalTransactions { get; set; }
        
        /// <summary>
        /// متوسط قيمة المعاملة
        /// </summary>
        [JsonPropertyName("averageTransactionValue")]
        public decimal AverageTransactionValue { get; set; }
        
        /// <summary>
        /// الإيرادات حسب اليوم
        /// </summary>
        [JsonPropertyName("revenueByDay")]
        public Dictionary<string, decimal> RevenueByDay { get; set; } = new Dictionary<string, decimal>();
        
        /// <summary>
        /// الإيرادات حسب خطة الاشتراك
        /// </summary>
        [JsonPropertyName("revenueByPlan")]
        public Dictionary<string, decimal> RevenueByPlan { get; set; } = new Dictionary<string, decimal>();
        
        /// <summary>
        /// توزيع طرق الدفع
        /// </summary>
        [JsonPropertyName("paymentMethodDistribution")]
        public Dictionary<string, int> PaymentMethodDistribution { get; set; } = new Dictionary<string, int>();
    }
    
    /// <summary>
    /// نموذج عدد الكلمات المفتاحية
    /// </summary>
    public class KeywordCountDTO
    {
        /// <summary>
        /// الكلمة المفتاحية
        /// </summary>
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; } = string.Empty;
        
        /// <summary>
        /// عدد مرات الظهور
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
    
    /// <summary>
    /// نموذج عدد الاستعلامات
    /// </summary>
    public class QueryCountDTO
    {
        /// <summary>
        /// نص الاستعلام
        /// </summary>
        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;
        
        /// <summary>
        /// عدد مرات الظهور
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
} 