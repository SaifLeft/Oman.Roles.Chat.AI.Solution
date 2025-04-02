namespace Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating file information
    /// </summary>
    public class UpdateFileInfoRequestDTO
    {
        /// <summary>
        /// New title for the file
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// New description for the file
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// New keywords for the file
        /// </summary>
        public List<string> Keywords { get; set; } = new List<string>();
    }
}