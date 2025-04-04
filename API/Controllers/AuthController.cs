using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;
using Models.DTOs.Authorization;
using Services;
using Services.Common;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserController> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;
        private readonly IGoogleAuthService _googleAuthService;

        public UserController(
            IUserService userService,
            IJwtService jwtService,
            ILogger<UserController> logger,
            ILocalizationService localizationService,
            IConfiguration configuration,
            IGoogleAuthService googleAuthService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
            _googleAuthService = googleAuthService;
        }

        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<UserDTO>))]
        public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterUserRequestDTO registrationDto)
        {
            // Existing email registration implementation
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            registrationDto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            registrationDto.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.RegisterUserAsync(registrationDto, language);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<UserDTO>))]
        public async Task<IActionResult> RegisterWithPhone([FromBody] UserPhoneRegistrationDto registrationDto)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            try {
                // Set IP address and user agent information
                registrationDto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                registrationDto.UserAgent = Request.Headers["User-Agent"].ToString();
                
                // Convert from UserPhoneRegistrationDto to UserPhoneRegistrationDTO
                var registrationDTO = new Models.DTOs.Authorization.UserPhoneRegistrationDTO
                {
                    PhoneNumber = long.Parse(registrationDto.PhoneNumber.Replace("+", "")),
                    Password = registrationDto.Password,
                    ConfirmationCode = registrationDto.ConfirmationCode
                };
                
                // Call the service method
                var result = await _userService.RegisterWithPhoneAsync(registrationDTO, language);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during phone registration: {Message}", ex.Message);
                return BadRequest(BaseResponse.FailureResponse(
                    _localizationService.GetMessage("RegistrationError", "Errors", language), 400));
            }
        }

        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<AuthResponse>))]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthDto authDto)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            
            try {
                // Get the redirect URI from configuration or use a default
                string redirectUri = _configuration["Authentication:Google:RedirectUri"] ?? "https://localhost";
                
                // Validate and process the Google token
                var userInfo = await _googleAuthService.AuthenticateAsync(authDto.Token, redirectUri);
                if (userInfo == null)
                    return Unauthorized(BaseResponse.FailureResponse(_localizationService.GetMessage("InvalidGoogleToken", "Errors", language), 401));
                
                // Generate JWT tokens
                var tokens = await _jwtService.GenerateJwtToken(userInfo.Id);
                return Ok(BaseResponse<LoginResponse>.SuccessResponse(tokens, 
                    _localizationService.GetMessage("GoogleAuthSuccess", "Messages", language)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google authentication");
                return Unauthorized(BaseResponse.FailureResponse(_localizationService.GetMessage("GoogleAuthError", "Errors", language), 401));
            }
        }

        /// <summary>
        /// تسجيل الدخول باستخدام اسم المستخدم وكلمة المرور
        /// </summary>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<LoginResponse>))]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // تعيين عنوان IP ومعلومات المستعرض
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.LoginAsync(request, language);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// تسجيل الدخول باستخدام رقم الهاتف وكلمة المرور
        /// </summary>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<LoginResponse>))]
        public async Task<IActionResult> LoginWithPhone([FromBody] LoginWithPhoneRequestDTO request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // تعيين عنوان IP ومعلومات المستعرض
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.LoginWithPhoneAsync(request, language);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// تحديث رمز JWT باستخدام رمز التحديث
        /// </summary>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<LoginResponse>))]

        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var response = await _jwtService.RefreshTokenAsync(request.RefreshToken);
            if (response == null)
            {
                var errorMessage = _localizationService.GetMessage("InvalidRefreshToken", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            var successMessage = _localizationService.GetMessage("TokenRefreshed", "Messages", language);
            return Ok(BaseResponse<LoginResponse>.SuccessResponse(response, successMessage));
        }

        /// <summary>
        /// الحصول على الملف الشخصي للمستخدم الحالي
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseResponse<UserDTO>))]

        public async Task<IActionResult> GetProfile()
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // استخراج معرف المستخدم من رمز JWT
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
        /// تحديث الملف الشخصي للمستخدم الحالي
        /// </summary>
        [Authorize]
        [HttpPut]
        [ProducesDefaultResponseType(typeof(BaseResponse<UserDTO>))]

        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequestDTO request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // استخراج معرف المستخدم من رمز JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            // تعيين عنوان IP ومعلومات المستعرض
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.UpdateUserProfileAsync(userId, request, language);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// تغيير كلمة المرور للمستخدم الحالي
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // استخراج معرف المستخدم من رمز JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                var errorMessage = _localizationService.GetMessage("UserIdRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            // تعيين عنوان IP ومعلومات المستعرض
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.ChangePasswordAsync(userId, request, language);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// إعادة تعيين كلمة المرور (نسيان كلمة المرور)
        /// </summary>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            // تعيين عنوان IP ومعلومات المستعرض
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _userService.ResetPasswordAsync(request, language);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// تفعيل حساب المستخدم (للمسؤولين فقط)
        /// </summary>
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> ActivateUser(long userId)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var result = await _userService.ActivateUserAsync(userId, language);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// تعطيل حساب المستخدم (للمسؤولين فقط)
        /// </summary>
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseResponse<bool>))]
        public async Task<IActionResult> DeactivateUser(long userId)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            var result = await _userService.DeactivateUserAsync(userId, language);
            return StatusCode(result.StatusCode, result);
        }
    }
}