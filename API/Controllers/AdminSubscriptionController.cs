using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Subscription;
using Models.DTOs.Subscription.Requests;
using Services;
using Services.Common;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    [ApiController]
    [Route("api/Admin/Subscription/[action]")]
    public class AdminSubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<AdminSubscriptionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public AdminSubscriptionController(
            ISubscriptionService subscriptionService,
            ILogger<AdminSubscriptionController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على جميع الاشتراكات
        /// Get all subscriptions
        /// </summary>
        /// <param name="page">رقم الصفحة</param>
        /// <param name="pageSize">حجم الصفحة</param>
        /// <returns>قائمة بالاشتراكات</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<UserSubscriptionDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSubscriptions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var result = await _subscriptionService.GetAllSubscriptionsAsync(page, pageSize, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscriptions");
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("SubscriptionsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على اشتراك محدد
        /// Get specific subscription
        /// </summary>
        /// <param name="id">معرف الاشتراك</param>
        /// <returns>تفاصيل الاشتراك</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<UserSubscriptionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubscription(string id)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var result = await _subscriptionService.GetSubscriptionByIdAsync(id, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription: {SubscriptionId}", id);
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("SubscriptionRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على اشتراكات مستخدم محدد
        /// Get subscriptions for a specific user
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <returns>قائمة باشتراكات المستخدم</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<UserSubscriptionDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSubscriptions(string userId)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var result = await _subscriptionService.GetUserSubscriptionsHistoryAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user subscriptions: {UserId}", userId);
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("UserSubscriptionsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إنشاء خطة اشتراك جديدة
        /// Create a new subscription plan
        /// </summary>
        /// <param name="request">بيانات خطة الاشتراك</param>
        /// <returns>خطة الاشتراك المنشأة</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<SubscriptionPlanDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanRequestDTO request)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _subscriptionService.CreateSubscriptionPlanAsync(request, adminId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("SubscriptionPlanCreationError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحديث خطة اشتراك
        /// Update a subscription plan
        /// </summary>
        /// <param name="id">معرف خطة الاشتراك</param>
        /// <param name="request">بيانات تحديث خطة الاشتراك</param>
        /// <returns>خطة الاشتراك المحدثة</returns>
        [HttpPut]
        [ProducesResponseType(typeof(BaseResponse<SubscriptionPlanDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSubscriptionPlan(string id, [FromBody] UpdateSubscriptionPlanRequestDTO request)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _subscriptionService.UpdateSubscriptionPlanAsync(id, request, adminId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("SubscriptionPlanUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إنشاء كوبون خصم جديد
        /// Create a new discount coupon
        /// </summary>
        /// <param name="request">بيانات كوبون الخصم</param>
        /// <returns>كوبون الخصم المنشأ</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<DiscountCouponDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDiscountCoupon([FromBody] CreateDiscountCouponRequestDTO request)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _subscriptionService.CreateDiscountCouponAsync(request, adminId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating discount coupon");
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("DiscountCouponCreationError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على جميع كوبونات الخصم
        /// Get all discount coupons
        /// </summary>
        /// <param name="page">رقم الصفحة</param>
        /// <param name="pageSize">حجم الصفحة</param>
        /// <returns>قائمة بكوبونات الخصم</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<DiscountCouponDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDiscountCoupons([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var result = await _subscriptionService.GetAllDiscountCouponsAsync(page, pageSize, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving discount coupons");
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("DiscountCouponsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحديث كوبون خصم
        /// Update a discount coupon
        /// </summary>
        /// <param name="id">معرف كوبون الخصم</param>
        /// <param name="request">بيانات تحديث كوبون الخصم</param>
        /// <returns>كوبون الخصم المحدث</returns>
        [HttpPut]
        [ProducesResponseType(typeof(BaseResponse<DiscountCouponDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDiscountCoupon(string id, [FromBody] UpdateDiscountCouponRequestDTO request)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _subscriptionService.UpdateDiscountCouponAsync(id, request, adminId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating discount coupon: {CouponId}", id);
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("DiscountCouponUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على تقارير الاشتراكات
        /// Get subscription reports
        /// </summary>
        /// <param name="startDate">تاريخ البداية</param>
        /// <param name="endDate">تاريخ النهاية</param>
        /// <returns>تقارير الاشتراكات</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<SubscriptionReportDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubscriptionReports([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var result = await _subscriptionService.GetSubscriptionReportsAsync(startDate, endDate, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription reports");
                string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
                var errorMessage = _localizationService.GetMessage("SubscriptionReportsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }
    }
}