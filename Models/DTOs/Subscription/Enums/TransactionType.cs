    

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