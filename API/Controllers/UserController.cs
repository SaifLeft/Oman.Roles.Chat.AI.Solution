using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;
using Models.DTOs.Authorization;
using Services;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserProfileController> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;

        public UserProfileController(
            IUserService userService,
            ILogger<UserProfileController> logger,
            ILocalizationService localizationService,
            IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<UserDTO>))]
        public async Task<IActionResult> GetProfile()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            var result = await _userService.GetUserProfileAsync(userId, language);
            return StatusCode(result.StatusCode, result);
        }
        
        /// <summary>
        /// Update user profile
        /// </summary>
        [Authorize]
        [HttpPut]
        [ProducesDefaultResponseType(typeof(BaseResponse<UserDTO>))]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.UpdateUserProfileAsync(userId, request, language);
            return StatusCode(result.StatusCode, result);
        }
        
        /// <summary>
        /// Change user password
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.ChangePasswordAsync(userId, request, language);
            return StatusCode(result.StatusCode, result);
        }
    }
} 