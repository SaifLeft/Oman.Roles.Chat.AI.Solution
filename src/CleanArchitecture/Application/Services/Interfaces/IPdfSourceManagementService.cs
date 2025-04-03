using Application.Common;
using Application.DTOs;

namespace Application.Services
{
    /// <summary>
    /// Service interface for PDF source file management operations
    /// </summary>
    public interface IPdfSourceManagementService
    {
        /// <summary>
        /// Upload a new PDF file
        /// </summary>
        /// <param name="fileName">Original name of the file</param>
        /// <param name="title">Descriptive title of the file</param>
        /// <param name="description">Detailed description of the file</param>
        /// <param name="fileContent">Binary content of the file</param>
        /// <param name="contentType">MIME type of the file</param>
        /// <param name="keywords">Keywords associated with the file</param>
        /// <param name="userId">ID of the user uploading the file</param>
        /// <param name="language">Language for localized messages</param>
        /// <returns>Information about the uploaded file</returns>
        Task<BaseResponse<DataFileDTO>> UploadPdfFileAsync(string fileName, string title, string description, byte[] fileContent, string contentType, List<string> keywords, long userId, string language);

        /// <summary>
        /// Get all available PDF files for a user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="language">Language for localized messages</param>
        /// <returns>List of PDF files</returns>
        Task<BaseResponse<List<DataFileDTO>>> GetAvailablePdfFilesAsync(long userId, string language);

        /// <summary>
        /// Get information about a specific PDF file
        /// </summary>
        /// <param name="fileId">ID of the file</param>
        /// <param name="language">Language for localized messages</param>
        /// <returns>Information about the file</returns>
        Task<BaseResponse<DataFileDTO>> GetPdfFileInfoAsync(long fileId, string language);

        /// <summary>
        /// Update information for a PDF file
        /// </summary>
        /// <param name="fileId">ID of the file</param>
        /// <param name="title">New title for the file</param>
        /// <param name="description">New description for the file</param>
        /// <param name="keywords">New keywords for the file</param>
        /// <param name="userId">ID of the user making the update</param>
        /// <param name="language">Language for localized messages</param>
        /// <returns>Updated information about the file</returns>
        Task<BaseResponse<DataFileDTO>> UpdatePdfFileInfoAsync(long fileId, string title, string description, List<string> keywords, long userId, string language);

        /// <summary>
        /// Delete a PDF file
        /// </summary>
        /// <param name="fileId">ID of the file</param>
        /// <param name="userId">ID of the user making the deletion</param>
        /// <param name="language">Language for localized messages</param>
        /// <returns>Result of the deletion operation</returns>
        Task<BaseResponse<bool>> DeletePdfFileAsync(long fileId, long userId, string language);

        /// <summary>
        /// Search for PDF files by keywords
        /// </summary>
        /// <param name="keywords">Keywords to search for</param>
        /// <param name="language">Language for localized messages</param>
        /// <returns>List of matching PDF files</returns>
        Task<BaseResponse<List<DataFileDTO>>> SearchPdfFilesByKeywordsAsync(List<string> keywords, string language);
    }
}