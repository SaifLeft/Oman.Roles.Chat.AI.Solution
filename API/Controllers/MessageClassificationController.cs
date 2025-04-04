using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;
using Services;
using Services.Common;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MessageClassificationController : ControllerBase
    {
        private readonly IMessageClassificationService _classificationService;
        private readonly ILogger<MessageClassificationController> _logger;
        private readonly ILocalizationService _localizationService;

        public MessageClassificationController(
            IMessageClassificationService classificationService,
            ILogger<MessageClassificationController> logger,
            ILocalizationService localizationService)
        {
            _classificationService = classificationService;
            _logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// تصنيف رسالة تلقائيًا
        /// </summary>
        /// <param name="messageId">معرف الرسالة</param>
        /// <param name="language">اللغة</param>
        /// <returns>معلومات التصنيف</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<MessageCategoryDTO>))]
        public async Task<IActionResult> ClassifyMessage(int messageId, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 401));
                }

                var result = await _classificationService.ClassifyMessageAsync(messageId, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تصنيف الرسالة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحليل مشاعر الرسالة
        /// </summary>
        /// <param name="messageId">معرف الرسالة</param>
        /// <param name="language">اللغة</param>
        /// <returns>معلومات تحليل المشاعر</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<MessageSentimentDTO>))]
        public async Task<IActionResult> AnalyzeSentiment(int messageId, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<MessageSentimentDTO>.FailureResponse(errorMessage, 401));
                }

                var result = await _classificationService.AnalyzeSentimentAsync(messageId, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحليل مشاعر الرسالة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<MessageSentimentDTO>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على فئات الرسالة
        /// </summary>
        /// <param name="messageId">معرف الرسالة</param>
        /// <param name="language">اللغة</param>
        /// <returns>فئات الرسالة</returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<List<MessageCategoryDTO>>))]
        public async Task<IActionResult> GetMessageCategories(int messageId, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<List<MessageCategoryDTO>>.FailureResponse(errorMessage, 401));
                }

                var result = await _classificationService.GetMessageCategoriesAsync(messageId, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على فئات الرسالة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<List<MessageCategoryDTO>>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تعيين فئة الرسالة يدويًا
        /// </summary>
        /// <param name="messageId">معرف الرسالة</param>
        /// <param name="categoryId">معرف الفئة</param>
        /// <param name="language">اللغة</param>
        /// <returns>معلومات التصنيف</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<MessageCategoryDTO>))]
        public async Task<IActionResult> SetMessageCategory(int messageId, int categoryId, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 401));
                }

                var result = await _classificationService.SetMessageCategoryAsync(messageId, categoryId, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تعيين فئة الرسالة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على جميع الفئات القانونية
        /// </summary>
        /// <param name="language">اللغة</param>
        /// <returns>قائمة الفئات القانونية</returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<List<LegalCategoryDTO>>))]
        public async Task<IActionResult> GetLegalCategories([FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<List<LegalCategoryDTO>>.FailureResponse(errorMessage, 401));
                }

                var result = await _classificationService.GetLegalCategoriesAsync(language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على الفئات القانونية");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<List<LegalCategoryDTO>>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على ملخص تصنيف الاستعلامات
        /// </summary>
        /// <param name="query">معلمات التاريخ واللغة</param>
        /// <returns>ملخص تصنيف الاستعلامات</returns>
        [HttpGet]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        [ProducesDefaultResponseType(typeof(BaseResponse<QueryCategorySummaryDTO>))]
        public async Task<IActionResult> GetClassificationSummary([FromQuery] AnalyticsPeriodQuery query)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", query.Language);
                    return Unauthorized(BaseResponse<QueryCategorySummaryDTO>.FailureResponse(errorMessage, 401));
                }

                // تعيين قيم افتراضية للتواريخ إذا لم يتم توفيرها
                var fromDate = query.FromDate == default ? DateTime.Now.AddDays(-30) : query.FromDate;
                var toDate = query.ToDate == default ? DateTime.Now : query.ToDate;

                var result = await _classificationService.GetCategorySummaryAsync(fromDate, toDate, query.Language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على ملخص تصنيف الاستعلامات");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", query.Language);
                return StatusCode(500, BaseResponse<QueryCategorySummaryDTO>.FailureResponse(errorMessage, 500));
            }
        }
    }
}