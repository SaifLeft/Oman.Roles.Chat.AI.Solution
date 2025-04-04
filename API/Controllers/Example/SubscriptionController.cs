// This file is temporarily commented out because it depends on ISubscriptionService methods
// that don't exist in the current implementation.
// Please update the ISubscriptionService interface to include the required methods.

/*
using API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Subscription.Requests;
using Services;
using System.Security.Claims;

namespace API.Controllers.Example
{
    /// <summary>
    /// تحكم الاشتراكات
    /// Controller for subscription operations
    /// </summary>
    [Authorize]
    public class SubscriptionController : ApiControllerBase
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
        [AllowAnonymous]
        public async Task<IActionResult> GetPlans()
        {
            try
            {
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.GetAllPlansAsync(language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plans");
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GenericError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الاشتراك في خطة
        /// Subscribe to a plan
        /// </summary>
        /// <param name="request">بيانات طلب الاشتراك</param>
        /// <returns>نتيجة عملية الاشتراك</returns>
        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                    var errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.CreateSubscription(userId, request, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to plan");
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GenericError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// التحقق من صلاحية كوبون
        /// Validate a coupon
        /// </summary>
        /// <param name="request">بيانات طلب التحقق من الكوبون</param>
        /// <returns>نتيجة التحقق من الكوبون</returns>
        [HttpPost]
        public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponRequest request)
        {
            try
            {
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.ValidateCouponAsync(request, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating coupon: {CouponCode}", request.CouponCode);
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GenericError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }
    }
}
*/