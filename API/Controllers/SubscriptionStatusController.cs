// This file is temporarily commented out because it depends on ISubscriptionStatusService
// which has been commented out due to entity property mismatches.
// Please update the entity properties or modify the service to match the actual database schema.

using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Subscription;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SubscriptionStatusController : ControllerBase
    {
        private readonly ISubscriptionStatusService _subscriptionStatusService;
        private readonly ILogger<SubscriptionStatusController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public SubscriptionStatusController(
            ISubscriptionStatusService subscriptionStatusService,
            ILogger<SubscriptionStatusController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _subscriptionStatusService = subscriptionStatusService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على حالة الاشتراك للمستخدم الحالي
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<SubscriptionStatusDTO>))]
        public async Task<IActionResult> GetStatus()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            // استخراج معرف المستخدم من الـ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }
            
            try
            {
                var result = await _subscriptionStatusService.GetUserSubscriptionStatusAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على حالة الاشتراك للمستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionStatusError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
        
        /// <summary>
        /// الحصول على استخدام الاستعلامات للمستخدم الحالي
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<QueryUsageDTO>))]
        public async Task<IActionResult> GetUsage()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            // استخراج معرف المستخدم من الـ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }
            
            try
            {
                var result = await _subscriptionStatusService.GetQueryUsageAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على استخدام الاستعلامات للمستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("QueryUsageError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
        
        /// <summary>
        /// التحقق من قدرة المستخدم على إجراء استعلام
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> CanMakeQuery()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            // استخراج معرف المستخدم من الـ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }
            
            try
            {
                var result = await _subscriptionStatusService.IsSubscriptionValidAsync(userId, language);
                
                if (result.Success && result.Data)
                {
                    var successMessage = _localizationService.GetMessage("CanMakeQuery", "Messages", language);
                    return Ok(BaseResponse<bool>.SuccessResponse(true, successMessage));
                }
                else
                {
                    var errorMessage = _localizationService.GetMessage("CannotMakeQuery", "Errors", language);
                    return Ok(BaseResponse<bool>.SuccessResponse(false, errorMessage));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء التحقق من قدرة المستخدم على إجراء استعلام {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("QueryCheckError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
        
        /// <summary>
        /// إرسال إشعارات انتهاء الاشتراك (للمسؤولين فقط)
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse))]
        public async Task<IActionResult> SendExpirationNotifications()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            try
            {
                // Reset monthly counters for all users
                var result = await _subscriptionStatusService.ResetMonthlyQueryCountersAsync(null, language);
                if (result.Success)
                {
                    var successMessage = _localizationService.GetMessage("CountersResetSuccessfully", "Messages", language);
                    return Ok(BaseResponse.SuccessResponse(successMessage));
                }
                else
                {
                    return StatusCode(result.StatusCode, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إعادة تعيين عدادات الاستعلامات الشهرية");
                var errorMessage = _localizationService.GetMessage("CountersResetError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }
}

/*
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Subscription;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SubscriptionStatusController : ControllerBase
    {
        private readonly ISubscriptionStatusService _subscriptionStatusService;
        private readonly ILogger<SubscriptionStatusController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public SubscriptionStatusController(
            ISubscriptionStatusService subscriptionStatusService,
            ILogger<SubscriptionStatusController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _subscriptionStatusService = subscriptionStatusService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على حالة الاشتراك للمستخدم الحالي
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<SubscriptionStatusDTO>))]
        public async Task<IActionResult> GetStatus()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            // استخراج معرف المستخدم من الـ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }
            
            try
            {
                var result = await _subscriptionStatusService.GetSubscriptionStatusAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على حالة الاشتراك للمستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionStatusError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
        
        /// <summary>
        /// الحصول على استخدام الاستعلامات للمستخدم الحالي
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<QueryUsageDTO>))]
        public async Task<IActionResult> GetUsage()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            // استخراج معرف المستخدم من الـ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }
            
            try
            {
                var result = await _subscriptionStatusService.GetQueryUsageAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على استخدام الاستعلامات للمستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("QueryUsageError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
        
        /// <summary>
        /// التحقق من قدرة المستخدم على إجراء استعلام
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> CanMakeQuery()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            // استخراج معرف المستخدم من الـ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }
            
            try
            {
                var canMakeQuery = await _subscriptionStatusService.CanMakeQueryAsync(userId);
                
                if (canMakeQuery)
                {
                    var successMessage = _localizationService.GetMessage("CanMakeQuery", "Messages", language);
                    return Ok(BaseResponse<bool>.SuccessResponse(true, successMessage));
                }
                else
                {
                    var errorMessage = _localizationService.GetMessage("CannotMakeQuery", "Errors", language);
                    return Ok(BaseResponse<bool>.SuccessResponse(false, errorMessage));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء التحقق من قدرة المستخدم على إجراء استعلام {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("QueryCheckError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
        
        /// <summary>
        /// إرسال إشعارات انتهاء الاشتراك (للمسؤولين فقط)
        /// </summary>
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse))]
        public async Task<IActionResult> SendExpirationNotifications()
        {
            // استخراج اللغة المفضلة من الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            try
            {
                await _subscriptionStatusService.SendExpirationNotificationsAsync();
                var successMessage = _localizationService.GetMessage("ExpirationNotificationsSent", "Messages", language);
                return Ok(BaseResponse.SuccessResponse(successMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إرسال إشعارات انتهاء الاشتراك");
                var errorMessage = _localizationService.GetMessage("ExpirationNotificationsError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }
}
*/ 