using Domain.Entities;

namespace Domain.Repositories
{
    /// <summary>
    /// Repository interface for DataSourceFile operations
    /// </summary>
    public interface IDataSourceFileRepository
    {
        /// <summary>
        /// Get a file by its ID
        /// </summary>
        /// <param name="id">The file ID</param>
        /// <returns>The file if found, null otherwise</returns>
        Task<DataSourceFile?> GetByIdAsync(long id);

        /// <summary>
        /// Get all files for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of files belonging to the user</returns>
        Task<IEnumerable<DataSourceFile>> GetByUserIdAsync(long userId);

        /// <summary>
        /// Add a new file
        /// </summary>
        /// <param name="file">The file to add</param>
        /// <returns>The added file with its ID</returns>
        Task<DataSourceFile> AddAsync(DataSourceFile file);

        /// <summary>
        /// Update an existing file
        /// </summary>
        /// <param name="file">The file with updated information</param>
        /// <returns>The updated file</returns>
        Task<DataSourceFile> UpdateAsync(DataSourceFile file);

        /// <summary>
        /// Delete a file by its ID
        /// </summary>
        /// <param name="id">The file ID</param>
        /// <returns>True if deleted, false otherwise</returns>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Search files by keywords
        /// </summary>
        /// <param name="keywords">List of keywords to search for</param>
        /// <returns>List of files matching any of the keywords</returns>
        Task<IEnumerable<DataSourceFile>> SearchByKeywordsAsync(IEnumerable<string> keywords);
    }
}