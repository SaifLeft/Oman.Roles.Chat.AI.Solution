using API.DTOs.Chat;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Services;
using Services.ModelService;
using System.Security.Claims;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DeepSeekController : ControllerBase
    {
        private readonly Services.ModelService.IDeepSeekService _deepSeekService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<DeepSeekController> _logger;
        private readonly IConfiguration _configuration;

        public DeepSeekController(
            Services.ModelService.IDeepSeekService deepSeekService,
            ILocalizationService localizationService,
            ILogger<DeepSeekController> logger,
            IConfiguration configuration)
        {
            _deepSeekService = deepSeekService;
            _localizationService = localizationService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// معالجة الاستعلام القانوني وتقديم استجابة
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesDefaultResponseType(typeof(BaseResponse<string>))]
        public async Task<IActionResult> ProcessQuery([FromBody] ChatQueryRequestDTO request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                _logger.LogInformation("تمت معالجة طلب استعلام من المستخدم {UserId}", userId);

                if (string.IsNullOrEmpty(request.Query))
                {
                    var errorMessage = _localizationService.GetMessage("EmptyQuery", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var result = await _deepSeekService.ProcessPdfDataAsync(request.Query, language);
                var successMessage = _localizationService.GetMessage("QueryProcessed", "Messages", language);
                return Ok(BaseResponse<string>.SuccessResponse(result, successMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء معالجة الاستعلام");
                var errorMessage = _localizationService.GetMessage("QueryProcessingError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إجراء استعلام قانوني مع نص سياق محدد
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesDefaultResponseType(typeof(BaseResponse<string>))]
        public async Task<IActionResult> LegalQuery([FromBody] LegalQueryRequestDTO request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                _logger.LogInformation("تمت معالجة طلب استعلام قانوني من المستخدم {UserId}", userId);

                if (string.IsNullOrEmpty(request.Query))
                {
                    var errorMessage = _localizationService.GetMessage("EmptyQuery", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var result = await _deepSeekService.ExecuteLegalQueryAsync(request.Query, request.Context ?? "", language);
                var successMessage = _localizationService.GetMessage("LegalQueryProcessed", "Messages", language);
                return Ok(BaseResponse<string>.SuccessResponse(result, successMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء معالجة الاستعلام القانوني");
                var errorMessage = _localizationService.GetMessage("LegalQueryError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على معلومات حول إجراء حكومي
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesDefaultResponseType(typeof(BaseResponse<string>))]
        public async Task<IActionResult> GovernmentProcedure([FromQuery] string service)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                _logger.LogInformation("تم طلب معلومات إجراء حكومي من المستخدم {UserId}", userId);

                if (string.IsNullOrEmpty(service))
                {
                    var errorMessage = _localizationService.GetMessage("EmptyServiceName", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var result = await _deepSeekService.GetGovernmentProcedureAsync(service, language);
                var successMessage = _localizationService.GetMessage("GovernmentProcedureRetrieved", "Messages", language);
                return Ok(BaseResponse<string>.SuccessResponse(result, successMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على معلومات الإجراء الحكومي");
                var errorMessage = _localizationService.GetMessage("GovernmentProcedureError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }
}