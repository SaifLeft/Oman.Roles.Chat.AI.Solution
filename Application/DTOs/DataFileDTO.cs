namespace Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for file information
    /// </summary>
    public class DataFileDTO
    {
        /// <summary>
        /// Unique identifier for the file
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Original name of the file
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Descriptive title of the file
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the file
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Physical path where the file is stored
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// MIME type of the file
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the file was created/uploaded
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// ID of the user who created/uploaded the file
        /// </summary>
        public long CreatedByUserId { get; set; }

        /// <summary>
        /// Collection of keywords associated with this file
        /// </summary>
        public List<string> Keywords { get; set; } = new List<string>();
    }
}