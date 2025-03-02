using API.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Muhami.DTOs;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ConversationAnalyticsController : ControllerBase
    {
        private readonly IConversationTrackingService _conversationTrackingService;
        private readonly ILogger<ConversationAnalyticsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public ConversationAnalyticsController(
            IConversationTrackingService conversationTrackingService,
            ILogger<ConversationAnalyticsController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _conversationTrackingService = conversationTrackingService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Get user's conversation history
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetUserConversationHistory([FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var conversations = await _conversationTrackingService.GetUserConversationsAsync(userIdClaim, limit, offset);
                return Ok(BaseResponse<List<ConversationTrackingDTO>>.SuccessResponse(conversations));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation history for user {UserId}", userIdClaim);
                var errorMessage = _localizationService.GetMessage("ConversationHistoryError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get room conversation history
        /// </summary>
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetRoomConversationHistory(string roomId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            if (string.IsNullOrEmpty(roomId))
            {
                var errorMessage = _localizationService.GetMessage("RoomIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var conversations = await _conversationTrackingService.GetRoomConversationsAsync(roomId, limit, offset);
                return Ok(BaseResponse<List<ConversationTrackingDTO>>.SuccessResponse(conversations));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation history for room {RoomId}", roomId);
                var errorMessage = _localizationService.GetMessage("ConversationHistoryError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get user's conversation analytics
        /// </summary>
        [HttpGet("user-analytics")]
        public async Task<IActionResult> GetUserAnalytics([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var analytics = await _conversationTrackingService.GetUserAnalyticsAsync(userIdClaim, fromDate, toDate);
                return Ok(BaseResponse<ConversationAnalyticsDTO>.SuccessResponse(analytics));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation analytics for user {UserId}", userIdClaim);
                var errorMessage = _localizationService.GetMessage("AnalyticsError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Get global conversation analytics (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("global-analytics")]
        public async Task<IActionResult> GetGlobalAnalytics([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var analytics = await _conversationTrackingService.GetAnalyticsAsync(fromDate, toDate);
                return Ok(BaseResponse<ConversationAnalyticsDTO>.SuccessResponse(analytics));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving global conversation analytics");
                var errorMessage = _localizationService.GetMessage("AnalyticsError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }
}