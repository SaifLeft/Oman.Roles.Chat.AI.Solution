using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Subscription;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<PaymentHistoryController> _logger;
        private readonly ILocalizationService _localizationService;

        public PaymentHistoryController(
            ISubscriptionService subscriptionService,
            ILogger<PaymentHistoryController> logger,
            ILocalizationService localizationService)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على سجل المدفوعات للمستخدم الحالي
        /// Get payment history for the current user
        /// </summary>
        /// <returns>سجل المدفوعات</returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<List<UserSubscriptionDTO>>))]
        public async Task<IActionResult> GetUserPaymentHistory([FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                    return BadRequest(BaseResponse<object>.FailureResponse(errorMessage, 400));
                }

                var result = await _subscriptionService.GetUserSubscriptionsHistoryAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history for user");
                var errorMessage = _localizationService.GetMessage("PaymentHistoryError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على تفاصيل دفعة محددة
        /// Get details for a specific payment
        /// </summary>
        /// <param name="paymentId">معرف الدفعة</param>
        /// <returns>تفاصيل الدفعة</returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<UserSubscriptionDTO>))]
        public async Task<IActionResult> GetPaymentDetails(string paymentId, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                    return BadRequest(BaseResponse<object>.FailureResponse(errorMessage, 400));
                }

                var result = await _subscriptionService.GetSubscriptionByIdAsync(paymentId, language);
                
                // Verify that the subscription belongs to the requesting user
                if (result.Success && result.Data.UserId.ToString() != userId)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedAccess", "Errors", language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }
                
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment details: {PaymentId}", paymentId);
                var errorMessage = _localizationService.GetMessage("PaymentDetailsError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }
    }
} 