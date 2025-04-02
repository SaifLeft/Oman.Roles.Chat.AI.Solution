using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
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
        private readonly IDeepSeekService _deepSeekService;

        public AdminController(
            IUserService userService,
            ISubscriptionService subscriptionService,
            IConversationTrackingService conversationTrackingService,
            IDeepSeekService deepSeekService,
            ILogger<AdminController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _userService = userService;
            _subscriptionService = subscriptionService;
            _conversationTrackingService = conversationTrackingService;
            _deepSeekService = deepSeekService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        #region User Management

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet("users")]
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
        [HttpGet("users/{id}")]
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
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] AdminUpdateUserRequest request)
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
        [HttpGet("subscriptions")]
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
        [HttpGet("subscription-plans")]
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
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _conversationTrackingService.GetAllConversationsAsync(page, pageSize, language);
                return StatusCode(result.StatusCode, result);
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
        [HttpGet("ai-models")]
        public async Task<IActionResult> GetAiModels()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _deepSeekService.GetAvailableModelsAsync(language);
                return StatusCode(result.StatusCode, result);
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
        [HttpPut("ai-models")]
        public async Task<IActionResult> UpdateAiModel([FromBody] UpdateAiModelRequest request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _deepSeekService.UpdateModelConfigurationAsync(request, language);
                return StatusCode(result.StatusCode, result);
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
        /// Get analytics data
        /// </summary>
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _conversationTrackingService.GetAnalyticsDataAsync(startDate, endDate, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analytics data");
                var errorMessage = _localizationService.GetMessage("AnalyticsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        #endregion
    }

    /// <summary>
    /// Admin update user request
    /// </summary>
    public class AdminUpdateUserRequest
    {
        /// <summary>
        /// User email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// User full name
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        public string? UserRole { get; set; }

        /// <summary>
        /// Is user active
        /// </summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Update AI model request
    /// </summary>
    public class UpdateAiModelRequest
    {
        /// <summary>
        /// Model name
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// API endpoint
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// API key
        /// </summary>
        public string? ApiKey { get; set; }
    }
}