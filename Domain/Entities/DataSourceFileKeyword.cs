namespace Domain.Entities
{
    /// <summary>
    /// Represents a keyword associated with a data source file
    /// </summary>
    public class DataSourceFileKeyword
    {
        /// <summary>
        /// Unique identifier for the keyword entry
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ID of the associated file
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// The keyword text
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property to the associated file
        /// </summary>
        public virtual DataSourceFile File { get; set; } = null!;
    }
}