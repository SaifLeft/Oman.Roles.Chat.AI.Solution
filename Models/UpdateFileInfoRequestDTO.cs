namespace Models
{
    public class UpdateFileInfoRequestDTO
    {
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
