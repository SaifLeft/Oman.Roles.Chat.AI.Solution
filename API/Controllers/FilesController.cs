using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Common;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FilesController : ControllerBase
    {
        private readonly IPdfSourceManagementService _pdfSourceManagementService;
        private readonly ILogger<FilesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public FilesController(
            IPdfSourceManagementService pdfSourceManagementService,
            ILogger<FilesController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _pdfSourceManagementService = pdfSourceManagementService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Upload a file (PDF or image)
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<DataFileDTO>))]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string title, [FromForm] string description, [FromForm] List<string> keywords)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // Get user ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            // Validate file
            if (file == null || file.Length == 0)
            {
                var errorMessage = _localizationService.GetMessage("FileRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            // Check file type (PDF or image)
            var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                var errorMessage = _localizationService.GetMessage("InvalidFileType", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                // Read file content
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                // Upload file
                var result = await _pdfSourceManagementService.UploadPdfFileAsync(
                    file.FileName,
                    title,
                    description,
                    fileContent,
                    file.ContentType,
                    keywords,
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
        /// Get file by ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFile([FromQuery] string id)
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
                _logger.LogError(ex, "Error retrieving file");
                var errorMessage = _localizationService.GetMessage("FileRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Update file information (title and description)
        /// </summary>
        [Authorize]
        [HttpPut]
        [ProducesDefaultResponseType(typeof(BaseResponse<DataFileDTO>))]
        public async Task<IActionResult> UpdateFileInfo([FromQuery] string id, [FromBody] UpdateFileInfoRequestDTO updateRequest)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // Get user ID from claims
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
                    updateRequest.Keywords, // Use keywords from request
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
        /// Delete file by ID
        /// </summary>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteFile([FromQuery] string id)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // Get user ID from claims
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
    }
}