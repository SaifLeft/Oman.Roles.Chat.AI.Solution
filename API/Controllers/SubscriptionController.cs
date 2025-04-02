using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using Models;
using System.Security.Claims;

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
        [HttpGet("plans")]
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
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
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
        public async Task<IActionResult> GetPlan(string id)
        {
            try
            {
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.GetPlanByIdAsync(id, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan: {PlanId}", id);
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
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
        public async Task<IActionResult> GetCurrentSubscription()
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                    var errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.GetUserSubscriptionAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user subscription");
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
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
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                    var errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                
                // Get the plan details
                var planResult = await _subscriptionService.GetPlanByIdAsync(request.PlanId, language);
                if (!planResult.Success)
                {
                    return StatusCode(planResult.StatusCode, planResult);
                }

                // Get user details
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "";

                // Calculate amount based on period type
                decimal amount = request.PeriodType == SubscriptionPeriodType.Monthly 
                    ? planResult.Data.PriceMonthly 
                    : planResult.Data.PriceYearly;

                // Apply coupon if provided
                if (!string.IsNullOrEmpty(request.CouponCode))
                {
                    var couponResult = await _subscriptionService.ValidateCouponAsync(
                        new ValidateCouponRequest { CouponCode = request.CouponCode, PlanId = request.PlanId },
                        language);

                    if (couponResult.Success && couponResult.Data.IsValid)
                    {
                        amount = couponResult.Data.DiscountedPrice;
                    }
                }

                // Create payment session
                var paymentResult = await _thawaniPaymentService.CreatePaymentSessionAsync(
                    amount,
                    userId,
                    userEmail,
                    userName,
                    request.PlanId,
                    language);

                return StatusCode(paymentResult.StatusCode, paymentResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription payment session");
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// التحقق من حالة الدفع
        /// Check payment status
        /// </summary>
        /// <param name="sessionId">معرف جلسة الدفع</param>
        /// <returns>حالة الدفع</returns>
        [Authorize]
        [HttpGet("payment-status/{sessionId}")]
        public async Task<IActionResult> CheckPaymentStatus(string sessionId)
        {
            try
            {
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _thawaniPaymentService.CheckPaymentStatusAsync(sessionId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status: {SessionId}", sessionId);
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إلغاء التجديد التلقائي للاشتراك
        /// Cancel subscription auto-renewal
        /// </summary>
        /// <param name="subscriptionId">معرف الاشتراك</param>
        /// <returns>حالة الاشتراك بعد الإلغاء</returns>
        [Authorize]
        [HttpPost("cancel-auto-renewal/{subscriptionId}")]
        public async Task<IActionResult> CancelAutoRenewal(string subscriptionId)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                    var errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.CancelAutoRenewalAsync(userId, subscriptionId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription auto-renewal: {SubscriptionId}", subscriptionId);
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إلغاء الاشتراك
        /// Cancel subscription
        /// </summary>
        /// <param name="subscriptionId">معرف الاشتراك</param>
        /// <returns>حالة الاشتراك بعد الإلغاء</returns>
        [Authorize]
        [HttpPost("cancel/{subscriptionId}")]
        public async Task<IActionResult> CancelSubscription(string subscriptionId)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                    var errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.CancelSubscriptionAsync(userId, subscriptionId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription: {SubscriptionId}", subscriptionId);
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على سجل المعاملات المالية للمستخدم
        /// Get user's transaction history
        /// </summary>
        /// <returns>قائمة بالمعاملات المالية</returns>
        [Authorize]
        [HttpGet("transaction-history")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                if (string.IsNullOrEmpty(userId))
                {
                    string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                    var errorMessage = _localizationService.GetMessage("Unauthorized", "Errors", language);
                    return StatusCode(401, BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _subscriptionService.GetUserTransactionsAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction history");
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// التحقق من صلاحية كوبون خصم
        /// Validate discount coupon
        /// </summary>
        /// <param name="request">بيانات طلب التحقق من الكوبون</param>
        /// <returns>نتيجة التحقق من الكوبون</returns>
        [HttpPost("validate-coupon")]
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
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        // Payment webhook handling has been moved to PaymentController
    }
}