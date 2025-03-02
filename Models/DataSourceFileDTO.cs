namespace Models
{
    public class DataSourceFileDTO
    {
        public long Id { get; set; }

        public string FileName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }
        /// <summary>
        /// bytes
        /// </summary>
        public long Size { get; set; }
        public string ContentType { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<string> Keywords { get; set; }
    }
}
