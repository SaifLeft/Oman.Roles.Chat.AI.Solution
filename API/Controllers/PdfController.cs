using API.Helpers;
using API.Services;
using API.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oman.Roles.Chat.AI.Models.Pdf;
using Oman.Roles.Chat.AI.Models.Responses;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly ILogger<PdfController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public PdfController(
            IPdfService pdfService,
            ILogger<PdfController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _pdfService = pdfService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على قائمة ملفات PDF المتاحة
        /// </summary>
        [HttpGet]
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
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على قائمة ملفات PDF");
                var errorMessage = _localizationService.GetMessage("PdfFilesRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على معلومات ملف PDF
        /// </summary>
        [HttpGet("{fileName}")]
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
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على معلومات ملف PDF");
                var errorMessage = _localizationService.GetMessage("PdfFileInfoRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحديث معلومات ملف PDF
        /// </summary>
        [Authorize]
        [HttpPut("{fileName}")]
        public async Task<IActionResult> UpdatePdfInfo(string fileName, [FromBody] UpdatePdfInfoRequest request)
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
                _logger.LogError(ex, "حدث خطأ أثناء تحديث معلومات ملف PDF");
                var errorMessage = _localizationService.GetMessage("PdfFileInfoUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }

    /// <summary>
    /// نموذج طلب تحديث معلومات ملف PDF
    /// </summary>
    public class UpdatePdfInfoRequest
    {
        /// <summary>
        /// العنوان الوصفي للملف
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// وصف الملف
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// الكلمات المفتاحية للملف
        /// </summary>
        public List<string> Keywords { get; set; } = new();
    }
}