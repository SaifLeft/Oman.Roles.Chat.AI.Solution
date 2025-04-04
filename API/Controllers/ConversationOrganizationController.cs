using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ConversationOrganizationController : ControllerBase
    {
        private readonly IConversationOrganizationService _organizationService;
        private readonly ILogger<ConversationOrganizationController> _logger;
        private readonly ILocalizationService _localizationService;

        public ConversationOrganizationController(
            IConversationOrganizationService organizationService,
            ILogger<ConversationOrganizationController> logger,
            ILocalizationService localizationService)
        {
            _organizationService = organizationService;
            _logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// إنشاء مجلد جديد للمحادثات
        /// </summary>
        /// <param name="folderName">اسم المجلد</param>
        /// <param name="parentFolderId">معرف المجلد الأب (اختياري)</param>
        /// <param name="language">اللغة</param>
        /// <returns>معلومات المجلد الجديد</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<ChatRoomFolderDTO>))]
        public async Task<IActionResult> CreateFolder([FromQuery] string folderName, [FromQuery] int? parentFolderId = null, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 401));
                }

                if (string.IsNullOrWhiteSpace(folderName))
                {
                    var errorMessage = _localizationService.GetMessage("FolderNameRequired", "Errors", language);
                    return BadRequest(BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 400));
                }

                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 401));
                }
                var result = await _organizationService.CreateFolderAsync(userIdLong, folderName, parentFolderId, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إنشاء مجلد جديد");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على قائمة المجلدات للمستخدم
        /// </summary>
        /// <param name="language">اللغة</param>
        /// <returns>قائمة المجلدات</returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<List<ChatRoomFolderDTO>>))]
        public async Task<IActionResult> GetUserFolders([FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<List<ChatRoomFolderDTO>>.FailureResponse(errorMessage, 401));
                }
                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<List<ChatRoomFolderDTO>>.FailureResponse(errorMessage, 401));
                }

                var result = await _organizationService.GetUserFoldersAsync(userIdLong, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على قائمة المجلدات");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<List<ChatRoomFolderDTO>>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// نقل محادثة إلى مجلد
        /// </summary>
        /// <param name="conversationId">معرف المحادثة</param>
        /// <param name="folderId">معرف المجلد (null لنقل إلى الجذر)</param>
        /// <param name="language">اللغة</param>
        /// <returns>نتيجة العملية</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> MoveConversationToFolder(int conversationId, [FromQuery] int? folderId = null, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }
                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }
                var result = await _organizationService.MoveConversationToFolderAsync(conversationId, folderId, userIdLong, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء نقل محادثة إلى مجلد");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<bool>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحديث عنوان المحادثة
        /// </summary>
        /// <param name="conversationId">معرف المحادثة</param>
        /// <param name="title">العنوان الجديد</param>
        /// <param name="language">اللغة</param>
        /// <returns>نتيجة العملية</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> UpdateConversationTitle(int conversationId, [FromQuery] string title, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    var errorMessage = _localizationService.GetMessage("TitleRequired", "Errors", language);
                    return BadRequest(BaseResponse<bool>.FailureResponse(errorMessage, 400));
                }

                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }

                var result = await _organizationService.UpdateConversationTitleAsync(conversationId, title, userIdLong, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث عنوان المحادثة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<bool>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحديث وسوم المحادثة
        /// </summary>
        /// <param name="conversationId">معرف المحادثة</param>
        /// <param name="tags">قائمة الوسوم</param>
        /// <param name="language">اللغة</param>
        /// <returns>نتيجة العملية</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> UpdateConversationTags(int conversationId, [FromBody] List<string> tags, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }
                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }
                var result = await _organizationService.UpdateConversationTagsAsync(conversationId, tags, userIdLong, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث وسوم المحادثة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<bool>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إضافة/إزالة المحادثة من المفضلة
        /// </summary>
        /// <param name="conversationId">معرف المحادثة</param>
        /// <param name="language">اللغة</param>
        /// <returns>نتيجة العملية</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> ToggleFavorite(int conversationId, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }
                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<object>.FailureResponse(errorMessage, 401));
                }
                var result = await _organizationService.ToggleFavoriteAsync(conversationId, userIdLong, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تبديل حالة المفضلة للمحادثة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<object>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحديث حالة المحادثة
        /// </summary>
        /// <param name="conversationId">معرف المحادثة</param>
        /// <param name="status">الحالة الجديدة</param>
        /// <param name="language">اللغة</param>
        /// <returns>نتيجة العملية</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> UpdateConversationStatus(int conversationId, [FromQuery] string status, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }

                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<bool>.FailureResponse(errorMessage, 401));
                }

                if (string.IsNullOrWhiteSpace(status))
                {
                    var errorMessage = _localizationService.GetMessage("StatusRequired", "Errors", language);
                    return BadRequest(BaseResponse<bool>.FailureResponse(errorMessage, 400));
                }

                var result = await _organizationService.UpdateConversationStatusAsync(conversationId, status, userIdLong, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث حالة المحادثة");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<bool>.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// البحث المتقدم في المحادثات
        /// </summary>
        /// <param name="query">معلمات البحث</param>
        /// <param name="language">اللغة</param>
        /// <returns>نتائج البحث</returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>))]
        public async Task<IActionResult> SearchConversations([FromBody] ConversationSearchQuery query, [FromQuery] string language = "ar")
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUser", "Errors", language);
                    return Unauthorized(BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>.FailureResponse(errorMessage, 401));
                }
                long userIdLong;
                if (!long.TryParse(userId, out userIdLong))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return Unauthorized(BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>.FailureResponse(errorMessage, 401));
                }
                // تعيين اللغة
                query.Language = language;

                var result = await _organizationService.SearchConversationsAsync(query, userIdLong, language);

                if (result.Success)
                {
                    return Ok(result);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء البحث في المحادثات");
                var errorMessage = _localizationService.GetMessage("ServerError", "Errors", language);
                return StatusCode(500, BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>.FailureResponse(errorMessage, 500));
            }
        }
    }
}