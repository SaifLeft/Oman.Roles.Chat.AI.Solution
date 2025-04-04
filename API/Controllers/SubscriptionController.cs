using API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using System.Security.Claims;
using Models.Common;
using Models.DTOs.Subscription;
using Models.DTOs.Subscription.Requests;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IThawaniPaymentService _thawaniPaymentService;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public SubscriptionController(
            ISubscriptionService subscriptionService,
            IThawaniPaymentService thawaniPaymentService,
            ILogger<SubscriptionController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _subscriptionService = subscriptionService;
            _thawaniPaymentService = thawaniPaymentService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على جميع خطط الاشتراك المتاحة
        /// Get all available subscription plans
        /// </summary>
        /// <returns>قائمة بخطط الاشتراك المتاحة</returns>
        [HttpGet]
        public async Task<IActionResult> GetSubscriptionPlans([FromQuery] string language = "ar")
        {
            try
            {
                var result = await _subscriptionService.GetAllPlansAsync(language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plans");
                string errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على خطة اشتراك محددة
        /// Get a specific subscription plan
        /// </summary>
        /// <param name="id">معرف خطة الاشتراك</param>
        /// <returns>تفاصيل خطة الاشتراك</returns>
        [HttpGet("plans/{id}")]
        public async Task<IActionResult> GetSubscriptionPlanById(int id, [FromQuery] string language = "ar")
        {
            try
            {
                var result = await _subscriptionService.GetPlanByIdAsync(id.ToString(), language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan: {PlanId}", id);
                string errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على اشتراك المستخدم الحالي
        /// Get current user's subscription
        /// </summary>
        /// <returns>تفاصيل اشتراك المستخدم</returns>
        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSubscription([FromQuery] string language = "ar")
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                var result = await _subscriptionService.GetUserSubscriptionAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user subscription");
                string errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إنشاء جلسة دفع للاشتراك
        /// Create a payment session for subscription
        /// </summary>
        /// <param name="request">بيانات طلب الاشتراك</param>
        /// <returns>معلومات جلسة الدفع</returns>
        [Authorize]
        [HttpPost("subscribe")]
        public async Task<IActionResult> SubscribeToPlan([FromBody] SubscriptionRequestDTO request, [FromQuery] string language = "ar")
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                // Get the plan details
                var planResult = await _subscriptionService.GetPlanByIdAsync(request.PlanId, language);
                if (!planResult.Success)
                {
                    return StatusCode(planResult.StatusCode, planResult);
                }

                // Get user details
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "";

                // Convert the SubscriptionRequestDTO to CreateSubscriptionRequestDTO
                var subscriptionRequest = new CreateSubscriptionRequestDTO
                {
                    PlanId = long.Parse(request.PlanId),
                    PeriodType = request.PeriodType.ToString(),
                    CouponCode = request.CouponCode,
                    AutoRenew = true
                };

                // Create the subscription directly through the service
                var subscriptionResult = await _subscriptionService.CreateSubscription(userId, subscriptionRequest, language);
                
                return StatusCode(subscriptionResult.StatusCode, subscriptionResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                string errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على تاريخ اشتراكات المستخدم
        /// Get user's subscription history
        /// </summary>
        /// <returns>تاريخ اشتراكات المستخدم</returns>
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetSubscriptionHistory([FromQuery] string language = "ar")
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                var result = await _subscriptionService.GetUserSubscriptionsHistoryAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription history");
                string errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }
    }
}