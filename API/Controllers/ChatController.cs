using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Security.Claims;
using System.Linq;
using Models.Common;

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
        private readonly int _maxImageUploads;
        private readonly int _maxPdfUploads;

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
            _maxImageUploads = _configuration.GetValue<int>("ChatSettings:MaxImageUploads");
            _maxPdfUploads = _configuration.GetValue<int>("ChatSettings:MaxPdfUploads");
        }

        /// <summary>
        /// ����� ���� ����� �����
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateChatRoom([FromBody] CreateChatRoomRequest request)
        {
            // ������� ����� ������� �� ��� �����
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // ������� ���� �������� �� ����� ������
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
                _logger.LogError(ex, "��� ��� ����� ����� ���� �����");
                var errorMessage = _localizationService.GetMessage("ChatRoomCreationError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// ������ ��� ���� ����� ������ ������
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetChatRoom(string roomId)
        {
            // ������� ����� ������� �� ��� �����
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            // ������� ���� �������� �� ����� ������
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
                _logger.LogError(ex, "��� ��� ����� ������ ��� ���� �������");
                var errorMessage = _localizationService.GetMessage("ChatRoomRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// ������ ��� ����� ��� ������� �������� ������
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserChatRooms()
        {
            // ������� ����� ������� �� ��� �����
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // ������� ���� �������� �� ����� ������
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
                _logger.LogError(ex, "��� ��� ����� ������ ��� ��� ������� ��������");
                var errorMessage = _localizationService.GetMessage("ChatRoomsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// ����� ����� �� ���� �����
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            // ������� ����� ������� �� ��� �����
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // ������� ���� �������� �� ����� ������
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
                _logger.LogError(ex, "��� ��� ����� ����� �������");
                var errorMessage = _localizationService.GetMessage("MessageSendingError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// ��� ���� �����
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteChatRoom(string roomId)
        {
            // ������� ����� ������� �� ��� �����
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // ������� ���� �������� �� ����� ������
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
                _logger.LogError(ex, "��� ��� ����� ��� ���� �������");
                var errorMessage = _localizationService.GetMessage("ChatRoomDeletionError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }
}