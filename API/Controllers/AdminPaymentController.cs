using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Payment;
using Models.DTOs.Subscription;
using Services;

namespace API.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminPaymentController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<AdminPaymentController> _logger;
        private readonly ILocalizationService _localizationService;

        public AdminPaymentController(
            ISubscriptionService subscriptionService,
            ILogger<AdminPaymentController> logger,
            ILocalizationService localizationService)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على جميع المدفوعات مع التصفح الصفحي
        /// Get all payments with pagination
        /// </summary>
        /// <param name="page">رقم الصفحة</param>
        /// <param name="pageSize">حجم الصفحة</param>
        /// <param name="language">اللغة</param>
        /// <returns>قائمة المدفوعات</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<UserSubscriptionDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPayments(int page = 1, int pageSize = 10, [FromQuery] string language = "ar")
        {
            try
            {
                var result = await _subscriptionService.GetAllSubscriptionsAsync(page, pageSize, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all payments with pagination: page {Page}, pageSize {PageSize}", page, pageSize);
                var errorMessage = _localizationService.GetMessage("PaymentsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على تقارير الدفع
        /// Get payment reports
        /// </summary>
        /// <param name="startDate">تاريخ البداية</param>
        /// <param name="endDate">تاريخ النهاية</param>
        /// <param name="language">اللغة</param>
        /// <returns>تقارير الدفع</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<PaymentReportDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentReports(DateTime? startDate = null, DateTime? endDate = null, [FromQuery] string language = "ar")
        {
            try
            {
                var subscriptionReport = await _subscriptionService.GetSubscriptionReportsAsync(startDate, endDate, language);

                if (!subscriptionReport.Success)
                {
                    return StatusCode(subscriptionReport.StatusCode, subscriptionReport);
                }

                // Create a more detailed payment report using the subscription data
                var report = new PaymentReportDTO
                {
                    TotalRevenue = subscriptionReport.Data.TotalRevenue,
                    TotalPayments = subscriptionReport.Data.TotalSubscriptions,
                    ActiveSubscribers = subscriptionReport.Data.ActiveSubscriptions,
                    AverageSubscriptionValue = subscriptionReport.Data.TotalSubscriptions > 0
                        ? subscriptionReport.Data.TotalRevenue / subscriptionReport.Data.TotalSubscriptions
                        : 0,
                    StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                    EndDate = endDate ?? DateTime.Now,
                    // Example calculation - this would be more complex in a real implementation
                    GrowthPercentage = 0
                };

                // Add plan distribution
                foreach (var plan in subscriptionReport.Data.SubscriptionsByPlan)
                {
                    report.SubscriptionPlanDistribution[plan.PlanName] = plan.SubscriptionCount;
                }

                var successMessage = _localizationService.GetMessage("PaymentReportsRetrieved", "Messages", language);
                return Ok(BaseResponse<PaymentReportDTO>.SuccessResponse(report, successMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment reports");
                var errorMessage = _localizationService.GetMessage("PaymentReportsError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تفاصيل دفعة محددة
        /// Get specific payment details
        /// </summary>
        /// <param name="id">معرف الدفعة</param>
        /// <param name="language">اللغة</param>
        /// <returns>تفاصيل الدفعة</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<UserSubscriptionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentDetails(string id, [FromQuery] string language = "ar")
        {
            try
            {
                var result = await _subscriptionService.GetSubscriptionByIdAsync(id, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment details: {PaymentId}", id);
                var errorMessage = _localizationService.GetMessage("PaymentDetailsError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }
    }
}