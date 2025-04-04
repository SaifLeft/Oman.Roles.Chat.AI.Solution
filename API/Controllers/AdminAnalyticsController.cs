using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Services;
using Services.Common;

namespace API.Controllers
{
    /// <summary>
    /// Defines custom claim type constants used throughout the application
    /// </summary>
    public static class CustomClaimTypes
    {
        /// <summary>
        /// Claim type for user ID
        /// </summary>
        public const string UserId = "userId";
    }

    [Route("api/Admin/Analytics/[action]")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly IAdminAnalyticsService _analyticsService;
        private readonly ILogger<AdminAnalyticsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public AdminAnalyticsController(
            IAdminAnalyticsService analyticsService,
            ILogger<AdminAnalyticsController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _analyticsService = analyticsService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على ملخص لوحة التحكم
        /// </summary>
        /// <param name="query">معلمات التاريخ واللغة</param>
        /// <returns>ملخص لوحة التحكم</returns>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] AnalyticsPeriodQuery query)
        {
            try
            {
                // استخراج معرف المستخدم من التوكن
                var userId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", query.Language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                // تعيين قيم افتراضية للتواريخ إذا لم يتم توفيرها
                var fromDate = query.FromDate == default ? DateTime.UtcNow.AddDays(-30) : query.FromDate;
                var toDate = query.ToDate == default ? DateTime.UtcNow : query.ToDate;

                var result = await _analyticsService.GetDashboardSummaryAsync(fromDate, toDate, query.Language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء محاولة الحصول على ملخص لوحة التحكم");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", query.Language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على إحصائيات المستخدمين
        /// </summary>
        /// <param name="query">معلمات التاريخ واللغة</param>
        /// <returns>إحصائيات المستخدمين</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetUserAnalytics([FromQuery] AnalyticsPeriodQuery query)
        {
            try
            {
                // استخراج معرف المستخدم من التوكن
                var userId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", query.Language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                // تعيين قيم افتراضية للتواريخ إذا لم يتم توفيرها
                var fromDate = query.FromDate == default ? DateTime.UtcNow.AddDays(-30) : query.FromDate;
                var toDate = query.ToDate == default ? DateTime.UtcNow : query.ToDate;

                var result = await _analyticsService.GetUserAnalyticsAsync(fromDate, toDate, query.Language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء محاولة الحصول على إحصائيات المستخدمين");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", query.Language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على إحصائيات الاشتراكات
        /// </summary>
        /// <param name="query">معلمات التاريخ واللغة</param>
        /// <returns>إحصائيات الاشتراكات</returns>
        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetSubscriptionAnalytics([FromQuery] AnalyticsPeriodQuery query)
        {
            try
            {
                // استخراج معرف المستخدم من التوكن
                var userId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", query.Language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                // تعيين قيم افتراضية للتواريخ إذا لم يتم توفيرها
                var fromDate = query.FromDate == default ? DateTime.UtcNow.AddDays(-30) : query.FromDate;
                var toDate = query.ToDate == default ? DateTime.UtcNow : query.ToDate;

                var result = await _analyticsService.GetSubscriptionAnalyticsAsync(fromDate, toDate, query.Language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء محاولة الحصول على إحصائيات الاشتراكات");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", query.Language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على إحصائيات الاستعلامات
        /// </summary>
        /// <param name="query">معلمات التاريخ واللغة</param>
        /// <returns>إحصائيات الاستعلامات</returns>
        [HttpGet("queries")]
        public async Task<IActionResult> GetQueryAnalytics([FromQuery] AnalyticsPeriodQuery query)
        {
            try
            {
                // استخراج معرف المستخدم من التوكن
                var userId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", query.Language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                // تعيين قيم افتراضية للتواريخ إذا لم يتم توفيرها
                var fromDate = query.FromDate == default ? DateTime.UtcNow.AddDays(-30) : query.FromDate;
                var toDate = query.ToDate == default ? DateTime.UtcNow : query.ToDate;

                var result = await _analyticsService.GetQueryAnalyticsAsync(fromDate, toDate, query.Language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء محاولة الحصول على إحصائيات الاستعلامات");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", query.Language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على إحصائيات المبيعات والإيرادات
        /// </summary>
        /// <param name="query">معلمات التاريخ واللغة</param>
        /// <returns>إحصائيات المبيعات والإيرادات</returns>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueAnalytics([FromQuery] AnalyticsPeriodQuery query)
        {
            try
            {
                // استخراج معرف المستخدم من التوكن
                var userId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", query.Language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }

                // تعيين قيم افتراضية للتواريخ إذا لم يتم توفيرها
                var fromDate = query.FromDate == default ? DateTime.UtcNow.AddDays(-30) : query.FromDate;
                var toDate = query.ToDate == default ? DateTime.UtcNow : query.ToDate;

                var result = await _analyticsService.GetRevenueAnalyticsAsync(fromDate, toDate, query.Language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء محاولة الحصول على إحصائيات المبيعات والإيرادات");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", query.Language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }
    }
}
