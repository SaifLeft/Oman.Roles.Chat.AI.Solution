namespace Models.DTOs
{
    /// <summary>
    /// نموذج نقل بيانات مجلد المحادثات
    /// </summary>
    public class ChatRoomFolderDTO
    {
        /// <summary>
        /// معرف المجلد
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// اسم المجلد
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// معرف المجلد الأب
        /// </summary>
        public long? ParentFolderId { get; set; }

        /// <summary>
        /// اسم المجلد الأب
        /// </summary>
        public string ParentFolderName { get; set; }

        /// <summary>
        /// المجلدات الفرعية
        /// </summary>
        public List<ChatRoomFolderDTO> SubFolders { get; set; }

        /// <summary>
        /// عدد المحادثات في المجلد
        /// </summary>
        public long ConversationCount { get; set; }
    }

    /// <summary>
    /// نموذج نقل بيانات محادثة مع معلومات تنظيمية
    /// </summary>
    public class OrganizedConversationDTO
    {
        /// <summary>
        /// معرف المحادثة
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// عنوان المحادثة
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// هل المحادثة في المفضلة
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// حالة المحادثة
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// وسوم المحادثة
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// مسار المجلد
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// تاريخ إنشاء المحادثة
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاريخ آخر نشاط
        /// </summary>
        public DateTime? LastActivity { get; set; }

        /// <summary>
        /// ملخص المحادثة
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// فئات الاستعلامات في هذه المحادثة
        /// </summary>
        public List<string> Categories { get; set; }
    }

    /// <summary>
    /// معلمات البحث المتقدم في المحادثات
    /// </summary>
    public class ConversationSearchQuery
    {
        /// <summary>
        /// نص البحث
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// البحث في المفضلة فقط
        /// </summary>
        public bool? IsFavorite { get; set; }

        /// <summary>
        /// البحث حسب حالة المحادثة
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// البحث حسب الوسوم
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// البحث حسب الفئات القانونية
        /// </summary>
        public List<int> CategoryIds { get; set; }

        /// <summary>
        /// البحث حسب المجلد
        /// </summary>
        public long? FolderId { get; set; }

        /// <summary>
        /// تاريخ البداية
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// تاريخ النهاية
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// ترتيب حسب (CreateDate, LastActivity, Title)
        /// </summary>
        public OrderByOptions OrderBy { get; set; } = OrderByOptions.LastActivity;

        /// <summary>
        /// ترتيب تنازلي
        /// </summary>
        public bool Descending { get; set; } = true;

        /// <summary>
        /// رقم الصفحة
        /// </summary>
        public long Page { get; set; } = 1;

        /// <summary>
        /// عدد العناصر في الصفحة
        /// </summary>
        public long PageSize { get; set; } = 10;
        public string Language { get; set; }
    }

    public enum OrderByOptions
    {
        CreateDate,
        Title,
        LastActivity
    }
}