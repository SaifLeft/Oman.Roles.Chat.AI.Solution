using API.Helpers;
using API.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;
using Models.DTOs.Files;
using Services;
using Services.Common;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private readonly IFileManagementService _fileManagementService;
        private readonly IPdfSourceManagementService _pdfSourceManagementService;
        private readonly IPdfService _pdfService;
        private readonly ISubscriptionStatusService _subscriptionStatusService;
        private readonly ILogger<FileController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public FileController(
            IFileManagementService fileManagementService,
            IPdfSourceManagementService pdfSourceManagementService,
            IPdfService pdfService,
            ISubscriptionStatusService subscriptionStatusService,
            ILogger<FileController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _fileManagementService = fileManagementService;
            _pdfSourceManagementService = pdfSourceManagementService;
            _pdfService = pdfService;
            _subscriptionStatusService = subscriptionStatusService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        #region File Upload Methods

        /// <summary>
        /// Upload a PDF file with binary content
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<Models.DTOs.Files.DataFileDTO>))]
        public async Task<IActionResult> UploadPdfBinary([FromBody] UploadFileRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long userIdLong))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Check subscription status
                var subscriptionStatus = await _subscriptionStatusService.GetUserSubscriptionStatusAsync(userIdLong, language);
                if (!subscriptionStatus.Success)
                {
                    return StatusCode(subscriptionStatus.StatusCode, subscriptionStatus);
                }

                // Verify it's a PDF
                if (!request.ContentType.ToLower().Contains("pdf"))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidFileType", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                // Create temp file
                var tempFilePath = Path.GetTempFileName();
                try
                {
                    // Write content to temp file
                    await System.IO.File.WriteAllBytesAsync(tempFilePath, request.FileContent);

                    // Use FileStream to upload
                    using var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);

                    // Create form file from temp file
                    var formFile = new FormFile(
                        fileStream,
                        0,
                        request.FileContent.Length,
                        null,
                        Path.GetFileName(request.FileName))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = request.ContentType
                    };

                    // Upload the file
                    var result = await _fileManagementService.UploadPdfFileAsync(formFile, userIdLong, language);

                    // Delete temp file
                    System.IO.File.Delete(tempFilePath);

                    return StatusCode(result.StatusCode, result);
                }
                catch (Exception)
                {
                    // Ensure temp file is deleted on error
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading PDF file");
                var errorMessage = _localizationService.GetMessage("FileUploadError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Upload an image with binary content
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<Models.DTOs.Files.DataFileDTO>))]
        public async Task<IActionResult> UploadImageBinary([FromBody] UploadFileRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long userIdLong))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Check subscription status
                var subscriptionStatus = await _subscriptionStatusService.GetUserSubscriptionStatusAsync(userIdLong, language);
                if (!subscriptionStatus.Success)
                {
                    return StatusCode(subscriptionStatus.StatusCode, subscriptionStatus);
                }

                // Verify it's an image
                if (!request.ContentType.ToLower().Contains("image"))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidFileType", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                // Create temp file
                var tempFilePath = Path.GetTempFileName();
                try
                {
                    // Write content to temp file
                    await System.IO.File.WriteAllBytesAsync(tempFilePath, request.FileContent);

                    // Use FileStream to upload
                    using var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);

                    // Create form file from temp file
                    var formFile = new FormFile(
                        fileStream,
                        0,
                        request.FileContent.Length,
                        null,
                        Path.GetFileName(request.FileName))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = request.ContentType
                    };

                    // Upload the file
                    var result = await _fileManagementService.UploadImageAsync(formFile, userIdLong, language);

                    // Delete temp file
                    System.IO.File.Delete(tempFilePath);

                    return StatusCode(result.StatusCode, result);
                }
                catch (Exception)
                {
                    // Ensure temp file is deleted on error
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                var errorMessage = _localizationService.GetMessage("ImageUploadError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Upload a file using form data (PDF or image)
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<PdfDocumentDTO>))]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Read file content
                using var memoryStream = new MemoryStream();
                await request.File.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                // Upload file
                var result = await _pdfSourceManagementService.UploadPdfFileAsync(
                    request.File.FileName,
                    request.Title,
                    request.Description,
                    fileContent,
                    request.File.ContentType,
                    request.Keywords,
                    long.Parse(userId),
                    language);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                var errorMessage = _localizationService.GetMessage("FileUploadError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Upload a PDF file to the knowledge base (Admin only)
        /// </summary>
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<PdfDocumentDTO>))]
        public async Task<IActionResult> UploadKnowledgeBasePdf([FromForm] FileUploadRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            // Verify admin role
            if (!User.IsInRole("Admin"))
            {
                var errorMessage = _localizationService.GetMessage("AdminRoleRequired", "Errors", language);
                return Forbid();
            }

            try
            {
                // Read file content
                using var memoryStream = new MemoryStream();
                await request.File.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                // Upload as knowledge base file
                var result = await _pdfSourceManagementService.UploadKnowledgeBasePdfAsync(
                    request.File.FileName,
                    request.Title,
                    request.Description,
                    fileContent,
                    request.File.ContentType,
                    request.Keywords,
                    long.Parse(userId),
                    language);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading knowledge base file");
                var errorMessage = _localizationService.GetMessage("KnowledgeBaseFileUploadError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion

        #region File Retrieval Methods

        /// <summary>
        /// Get file by ID
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<Models.DTOs.Files.DataFileDTO>))]
        public async Task<IActionResult> GetFile(string fileId)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long userIdLong))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Get file
                var result = await _fileManagementService.GetFileByIdAsync(fileId, userIdLong, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file information");
                var errorMessage = _localizationService.GetMessage("FileRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Download file by ID
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(FileContentResult))]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long userIdLong))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Get file info
                var fileInfoResult = await _fileManagementService.GetFileByIdAsync(fileId, userIdLong, language);
                if (!fileInfoResult.Success)
                {
                    return StatusCode(fileInfoResult.StatusCode, fileInfoResult);
                }

                // Get file content
                var fileContentResult = await _fileManagementService.GetFileContentAsync(fileId, userIdLong, language);
                if (!fileContentResult.Success)
                {
                    return StatusCode(fileContentResult.StatusCode, fileContentResult);
                }

                // Determine MIME type
                string contentType = fileInfoResult.Data.FileName.ToLower().EndsWith(".pdf")
                    ? "application/pdf"
                    : $"image/{GetImageExtension(fileInfoResult.Data.FileName)}";

                return File(fileContentResult.Data, contentType, fileInfoResult.Data.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                var errorMessage = _localizationService.GetMessage("FileDownloadError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get PDF file by ID
        /// </summary>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<PdfDocumentDTO>))]
        public async Task<IActionResult> GetPdfFile([FromQuery] string id)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                if (!long.TryParse(id, out long fileId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidFileId", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var result = await _pdfSourceManagementService.GetPdfFileInfoAsync(fileId, language);
                if (result.StatusCode != 200)
                {
                    return StatusCode(result.StatusCode, result);
                }

                // Return file content
                var filePath = result.Data.FilePath;
                if (!System.IO.File.Exists(filePath))
                {
                    var errorMessage = _localizationService.GetMessage("FileNotFound", "Errors", language);
                    return NotFound(BaseResponse.FailureResponse(errorMessage, 404));
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, result.Data.ContentType, result.Data.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PDF file");
                var errorMessage = _localizationService.GetMessage("FileRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get user files
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(PaginatedResponse<List<Models.DTOs.Files.DataFileDTO>>))]
        public async Task<IActionResult> GetUserFiles([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long userIdLong))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Get user files
                var result = await _fileManagementService.GetUserFilesAsync(userIdLong, page, pageSize, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user files");
                var errorMessage = _localizationService.GetMessage("UserFilesRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get available PDF files
        /// </summary>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<List<Models.DTOs.Files.DataFileDTO>>))]
        public async Task<IActionResult> GetAvailablePdfFiles()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _pdfService.GetAvailablePdfFilesAsync(language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PDF files list");
                var errorMessage = _localizationService.GetMessage("PdfFilesRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get PDF file information
        /// </summary>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<PdfDocumentDTO>))]
        public async Task<IActionResult> GetPdfFileInfo(string fileName)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _pdfService.GetPdfFileInfoAsync(fileName, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PDF file information");
                var errorMessage = _localizationService.GetMessage("PdfFileInfoRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion

        #region File Update/Delete Methods

        /// <summary>
        /// Update PDF file information
        /// </summary>
        [Authorize]
        [HttpPut]
        [ProducesDefaultResponseType(typeof(BaseResponse<PdfDocumentDTO>))]
        public async Task<IActionResult> UpdatePdfInfo(string fileName, [FromBody] UpdateFileInfoRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var result = await _pdfService.UpdatePdfInfoAsync(
                    fileName,
                    request.Title,
                    request.Description,
                    request.Keywords,
                    userId,
                    language);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PDF file information");
                var errorMessage = _localizationService.GetMessage("PdfFileInfoUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Update file information
        /// </summary>
        [Authorize]
        [HttpPut]
        [ProducesDefaultResponseType(typeof(BaseResponse<PdfDocumentDTO>))]
        public async Task<IActionResult> UpdateFileInfo([FromQuery] string id, [FromBody] UpdateFileInfoRequestDTO updateRequest)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                if (!long.TryParse(id, out long fileId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidFileId", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                // Validate request
                if (updateRequest == null || (string.IsNullOrEmpty(updateRequest.Title) && string.IsNullOrEmpty(updateRequest.Description)))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUpdateRequest", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                // Update file information
                var result = await _pdfSourceManagementService.UpdatePdfFileInfoAsync(
                    fileId,
                    updateRequest.Title,
                    updateRequest.Description,
                    updateRequest.Keywords,
                    long.Parse(userId),
                    language);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating file information");
                var errorMessage = _localizationService.GetMessage("FileUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        [Authorize]
        [HttpDelete]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long userIdLong))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Delete file
                var result = await _fileManagementService.DeleteFileAsync(fileId, userIdLong, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                var errorMessage = _localizationService.GetMessage("FileDeletionError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Delete PDF file by ID
        /// </summary>
        [Authorize]
        [HttpDelete]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> DeletePdfFile([FromQuery] string id)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                if (!long.TryParse(id, out long fileId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidFileId", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var result = await _pdfSourceManagementService.DeletePdfFileAsync(fileId, long.Parse(userId), language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                var errorMessage = _localizationService.GetMessage("FileDeleteError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get image extension from filename
        /// </summary>
        private string GetImageExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant().TrimStart('.');

            // Verify extension is a valid image type
            switch (extension)
            {
                case "jpg":
                case "jpeg":
                    return "jpeg";
                case "png":
                    return "png";
                case "gif":
                    return "gif";
                case "webp":
                    return "webp";
                default:
                    return "jpeg"; // Default
            }
        }

        #endregion
    }
}