namespace Models
{
    public class UpdateFileInfoRequestDTO
    {
        /// <summary>
        /// معرف الملف
        /// </summary>
        public long FileId { get; set; }
        
        /// <summary>
        /// عنوان الملف
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// وصف الملف
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// الكلمات المفتاحية المرتبطة بالملف
        /// </summary>
        public List<string> Keywords { get; set; }
    }
}
