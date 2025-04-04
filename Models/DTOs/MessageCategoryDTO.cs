namespace Models.DTOs
{
    /// <summary>
    /// نموذج نقل بيانات الفئة القانونية
    /// </summary>
    public class LegalCategoryDTO
    {
        /// <summary>
        /// معرف الفئة
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// اسم الفئة
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// وصف الفئة
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// نموذج نقل بيانات تصنيف الرسائل
    /// </summary>
    public class MessageCategoryDTO
    {
        /// <summary>
        /// معرف التصنيف
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// معرف الرسالة
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// معرف الفئة
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// اسم الفئة
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// درجة الثقة في التصنيف (0-1)
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// هل تم التصنيف تلقائيًا
        /// </summary>
        public bool IsAutoClassified { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// نموذج نقل بيانات تحليل المشاعر للرسالة
    /// </summary>
    public class MessageSentimentDTO
    {
        /// <summary>
        /// معرف الرسالة
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// درجة المشاعر (-1 سلبي للغاية إلى 1 إيجابي للغاية)
        /// </summary>
        public float SentimentScore { get; set; }

        /// <summary>
        /// درجة الإلحاح (1=منخفض، 2=متوسط، 3=عالي)
        /// </summary>
        public long Urgency { get; set; }

        /// <summary>
        /// النبرة العاطفية (قلق، غضب، إحباط، إلخ)
        /// </summary>
        public string EmotionalTone { get; set; }
    }

    /// <summary>
    /// إحصائيات حول تصنيف الاستعلامات
    /// </summary>
    public class QueryCategorySummaryDTO
    {
        /// <summary>
        /// توزيع الاستعلامات حسب الفئة القانونية
        /// </summary>
        public Dictionary<string, int> DistributionByCategory { get; set; }

        /// <summary>
        /// متوسط درجة الثقة في التصنيف
        /// </summary>
        public float AverageConfidence { get; set; }

        /// <summary>
        /// عدد الاستعلامات المصنفة تلقائيًا
        /// </summary>
        public long AutoClassifiedCount { get; set; }

        /// <summary>
        /// عدد الاستعلامات المصنفة يدويًا
        /// </summary>
        public long ManuallyClassifiedCount { get; set; }
    }
}