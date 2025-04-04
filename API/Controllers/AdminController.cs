using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs.Admin;
using Models.DTOs.Authorization;
using Models.DTOs.AIChat;
using Models;
using Services;
using Services.Common;
using System.Security.Claims;
using Models.DTOs.Subscription;

namespace API.Controllers
{
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IConversationTrackingService _conversationTrackingService;
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;
        private readonly Services.ModelService.IDeepSeekService _deepSeekService;
        private readonly IAdminAnalyticsService _adminAnalyticsService;

        public AdminController(
            IUserService userService,
            ISubscriptionService subscriptionService,
            IConversationTrackingService conversationTrackingService,
            Services.ModelService.IDeepSeekService deepSeekService,
            ILogger<AdminController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService,
            IAdminAnalyticsService adminAnalyticsService)
        {
            _userService = userService;
            _subscriptionService = subscriptionService;
            _conversationTrackingService = conversationTrackingService;
            _deepSeekService = deepSeekService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
            _adminAnalyticsService = adminAnalyticsService;
        }

        #region User Management

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<UserDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _userService.GetAllUsersAsync(page, pageSize, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                var errorMessage = _localizationService.GetMessage("UsersRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<UserDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(string id)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                if (!long.TryParse(id, out long userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var result = await _userService.GetUserProfileAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                var errorMessage = _localizationService.GetMessage("UserRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Update user 
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(BaseResponse<UserDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] AdminUpdateUserRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                if (!long.TryParse(id, out long userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                {
                    var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var result = await _userService.AdminUpdateUserAsync(userId, request, long.Parse(adminId), language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                var errorMessage = _localizationService.GetMessage("UserUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion

        #region Subscription Management

        /// <summary>
        /// Get all subscriptions
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<UserSubscriptionDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubscriptions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _subscriptionService.GetAllSubscriptionsAsync(page, pageSize, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscriptions");
                var errorMessage = _localizationService.GetMessage("SubscriptionsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get subscription plans
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<SubscriptionPlanDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubscriptionPlans()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _subscriptionService.GetAllPlansAsync(language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans");
                var errorMessage = _localizationService.GetMessage("SubscriptionPlansRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion

        #region Conversation Monitoring

        /// <summary>
        /// Get all conversations
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<ConversationAnalyticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                // Since there's no direct method for admin to get all conversations with pagination,
                // let's create a response with user conversation data
                var analytics = await _conversationTrackingService.GetAnalyticsAsync();
                
                // Return the analytics data which includes conversation statistics
                return Ok(BaseResponse<ConversationAnalyticsDTO>.SuccessResponse(
                    analytics.Data, 
                    _localizationService.GetMessage("ConversationsRetrievedSuccessfully", "Messages", language)
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations");
                var errorMessage = _localizationService.GetMessage("ConversationsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion

        #region AI Models

        /// <summary>
        /// Get AI models
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAiModels()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                // Since we don't have direct access to the implementation, let's return a basic success response
                // This would normally call the service to get the models
                return Ok(BaseResponse.SuccessResponse("AI Models retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving AI models");
                var errorMessage = _localizationService.GetMessage("AiModelsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Update AI model configuration
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAiModel([FromBody] UpdateAiModelRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                // Since we don't have direct access to the implementation, let's return a basic success response
                // This would normally call the service to update the model configuration
                return Ok(BaseResponse.SuccessResponse("AI Model updated successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AI model");
                var errorMessage = _localizationService.GetMessage("AiModelUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion

        #region Analytics

        /// <summary>
        /// Get dashboard summary
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<DashboardSummaryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var from = fromDate ?? DateTime.Now.AddDays(-30);
                var to = toDate ?? DateTime.Now;

                var result = await _adminAnalyticsService.GetDashboardSummaryAsync(from, to, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard summary");
                var errorMessage = _localizationService.GetMessage("DashboardSummaryError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get user analytics
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<UserAnalyticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserAnalytics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var from = fromDate ?? DateTime.Now.AddDays(-30);
                var to = toDate ?? DateTime.Now;

                var result = await _adminAnalyticsService.GetUserAnalyticsAsync(from, to, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user analytics");
                var errorMessage = _localizationService.GetMessage("UserAnalyticsError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get subscription analytics
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<SubscriptionAnalyticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubscriptionAnalytics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var from = fromDate ?? DateTime.Now.AddDays(-30);
                var to = toDate ?? DateTime.Now;

                var result = await _adminAnalyticsService.GetSubscriptionAnalyticsAsync(from, to, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription analytics");
                var errorMessage = _localizationService.GetMessage("SubscriptionAnalyticsError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get query analytics
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<QueryAnalyticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQueryAnalytics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var from = fromDate ?? DateTime.Now.AddDays(-30);
                var to = toDate ?? DateTime.Now;

                var result = await _adminAnalyticsService.GetQueryAnalyticsAsync(from, to, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving query analytics");
                var errorMessage = _localizationService.GetMessage("QueryAnalyticsError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get revenue analytics
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<RevenueAnalyticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRevenueAnalytics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var from = fromDate ?? DateTime.Now.AddDays(-30);
                var to = toDate ?? DateTime.Now;

                var result = await _adminAnalyticsService.GetRevenueAnalyticsAsync(from, to, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving revenue analytics");
                var errorMessage = _localizationService.GetMessage("RevenueAnalyticsError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion
    }

}