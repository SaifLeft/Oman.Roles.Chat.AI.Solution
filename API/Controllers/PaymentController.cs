using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Subscription;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        private readonly IThawaniPaymentService _thawaniPaymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly ILocalizationService _localizationService;

        public PaymentController(
            IThawaniPaymentService thawaniPaymentService,
            ILogger<PaymentController> logger,
            ILocalizationService localizationService)
        {
            _thawaniPaymentService = thawaniPaymentService;
            _logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// بدء عملية دفع جديدة
        /// Initialize a new payment process
        /// </summary>
        /// <param name="request">بيانات طلب الدفع</param>
        /// <returns>معلومات جلسة الدفع</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<ThawaniSessionResponse>))]
        public async Task<IActionResult> InitializePayment([FromBody] PaymentInitRequest request)
        {
            try
            {
                _logger.LogInformation("Initializing payment for user: {UserId}", request.UserId);
                
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                
                var result = await _thawaniPaymentService.CreatePaymentSessionAsync(
                    request.Amount,
                    request.UserId,
                    request.Email,
                    request.Name,
                    request.SubscriptionPlanId,
                    language);
                
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing payment");
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<ThawaniSessionResponse>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// معالجة إشعارات الدفع من بوابة ثواني
        /// Handle payment webhooks from Thawani payment gateway
        /// </summary>
        /// <returns>نتيجة معالجة الإشعار</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                _logger.LogInformation("Received payment webhook notification");
                
                // Read the request body
                using var reader = new StreamReader(Request.Body);
                var payload = await reader.ReadToEndAsync();

                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _thawaniPaymentService.HandlePaymentWebhookAsync(payload, language);
                
                _logger.LogInformation("Payment webhook processed with status: {Status}", result.Success);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment webhook");
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<bool>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// التحقق من حالة الدفع
        /// Check payment status
        /// </summary>
        /// <param name="sessionId">معرف جلسة الدفع</param>
        /// <returns>حالة الدفع</returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<ThawaniPaymentStatusResponse>))]
        public async Task<IActionResult> CheckPaymentStatus(string sessionId)
        {
            try
            {
                _logger.LogInformation("Checking payment status for session: {SessionId}", sessionId);
                
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var result = await _thawaniPaymentService.CheckPaymentStatusAsync(sessionId, language);
                
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status: {SessionId}", sessionId);
                string language = Request.Headers["Accept-Language"].ToString() ?? "en";
                var errorMessage = _localizationService.GetMessage("GeneralError", "Errors", language);
                return StatusCode(500, BaseResponse<ThawaniPaymentStatusResponse>.FailureResponse(errorMessage, 500));
            }
        }
    }

    public class PaymentInitRequest
    {
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string SubscriptionPlanId { get; set; }
    }
}