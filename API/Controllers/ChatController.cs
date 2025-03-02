using API.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatAIService _chatService;
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public ChatController(
            IChatAIService chatService,
            ILogger<ChatController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _chatService = chatService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// ≈‰‘«¡ €—›… œ—œ‘… ÃœÌœ…
        /// </summary>
        [HttpPost("rooms")]
        public async Task<IActionResult> CreateChatRoom([FromBody] CreateChatRoomRequest request)
        {
            // «” Œ—«Ã «··€… «·„›÷·… „‰ —√” «·ÿ·»
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // «” Œ—«Ã „⁄—› «·„” Œœ„ „‰ «·—„“ «·„„Ì“
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var result = await _chatService.CreateChatRoomAsync(request, userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ÕœÀ Œÿ√ √À‰«¡ ≈‰‘«¡ €—›… œ—œ‘…");
                var errorMessage = _localizationService.GetMessage("ChatRoomCreationError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// «·Õ’Ê· ⁄·Ï €—›… œ—œ‘… »Ê«”ÿ… «·„⁄—›
        /// </summary>
        [HttpGet("rooms/{roomId}")]
        public async Task<IActionResult> GetChatRoom(string roomId)
        {
            // «” Œ—«Ã «··€… «·„›÷·… „‰ —√” «·ÿ·»
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            // «” Œ—«Ã „⁄—› «·„” Œœ„ „‰ «·—„“ «·„„Ì“
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }
            try
            {

                var result = await _chatService.GetChatRoomAsync(roomId, userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ÕœÀ Œÿ√ √À‰«¡ «·Õ’Ê· ⁄·Ï €—›… «·œ—œ‘…");
                var errorMessage = _localizationService.GetMessage("ChatRoomRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// «·Õ’Ê· ⁄·Ï ﬁ«∆„… €—› «·œ—œ‘… ··„” Œœ„ «·Õ«·Ì
        /// </summary>
        [HttpGet("rooms")]
        public async Task<IActionResult> GetUserChatRooms()
        {
            // «” Œ—«Ã «··€… «·„›÷·… „‰ —√” «·ÿ·»
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // «” Œ—«Ã „⁄—› «·„” Œœ„ „‰ «·—„“ «·„„Ì“
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var result = await _chatService.GetUserChatRoomsAsync(userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ÕœÀ Œÿ√ √À‰«¡ «·Õ’Ê· ⁄·Ï €—› «·œ—œ‘… ··„” Œœ„");
                var errorMessage = _localizationService.GetMessage("ChatRoomsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// ≈—”«· —”«·… ›Ì €—›… œ—œ‘…
        /// </summary>
        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            // «” Œ—«Ã «··€… «·„›÷·… „‰ —√” «·ÿ·»
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // «” Œ—«Ã „⁄—› «·„” Œœ„ „‰ «·—„“ «·„„Ì“
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var result = await _chatService.SendMessageAsync(request, userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ÕœÀ Œÿ√ √À‰«¡ ≈—”«· «·—”«·…");
                var errorMessage = _localizationService.GetMessage("MessageSendingError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// Õ–› €—›… œ—œ‘…
        /// </summary>
        [HttpDelete("rooms/{roomId}")]
        public async Task<IActionResult> DeleteChatRoom(string roomId)
        {
            // «” Œ—«Ã «··€… «·„›÷·… „‰ —√” «·ÿ·»
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // «” Œ—«Ã „⁄—› «·„” Œœ„ „‰ «·—„“ «·„„Ì“
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var result = await _chatService.DeleteChatRoomAsync(roomId, userId, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ÕœÀ Œÿ√ √À‰«¡ Õ–› €—›… «·œ—œ‘…");
                var errorMessage = _localizationService.GetMessage("ChatRoomDeletionError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }
}