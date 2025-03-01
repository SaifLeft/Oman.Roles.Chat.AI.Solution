using API.Services;
using Helpers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IJwtService jwtService,
            ILogger<AuthController> logger,
            ILocalizationService localizationService,
            IConfiguration configuration)
        {
            _jwtService = jwtService;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
        }

        /// <summary>
        /// تسجيل الدخول واستلام رمز JWT
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            {
                // في بيئة حقيقية، ستقوم بالتحقق من بيانات المستخدم في قاعدة البيانات
                // هنا نقوم بعملية تسجيل دخول بسيطة لأغراض التوضيح
                if (request.Username == "admin" && request.Password == "password123")
                {
                    var user = new UserInfo
                    {
                        Id = "1",
                        Username = "admin",
                        Email = "admin@example.com",
                        FullName = "مدير النظام",
                        Roles = new List<string> { "Admin" }
                    };

                    var response = _jwtService.GenerateJwtToken(user);
                    var successMessage = _localizationService.GetMessage("LoginSuccess", "Messages", language);
                    return Ok(BaseResponse<LoginResponse>.SuccessResponse(response, successMessage));
                }
                else if (request.Username == "user" && request.Password == "password123")
                {
                    var user = new UserInfo
                    {
                        Id = "2",
                        Username = "user",
                        Email = "user@example.com",
                        FullName = "مستخدم عادي",
                        Roles = new List<string> { "User" }
                    };

                    var response = _jwtService.GenerateJwtToken(user);
                    var successMessage = _localizationService.GetMessage("LoginSuccess", "Messages", language);
                    return Ok(BaseResponse<LoginResponse>.SuccessResponse(response, successMessage));
                }

                var errorMessage = _localizationService.GetMessage("InvalidCredentials", "Errors", language);
                return Unauthorized(BaseResponse.FailureResponse(errorMessage, 401));
            }
        }

        /// <summary>
        /// تحديث رمز JWT باستخدام رمز التحديث
        /// </summary>
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            {
                var response = _jwtService.RefreshToken(request.RefreshToken);
                if (response == null)
                {
                    var errorMessage = _localizationService.GetMessage("InvalidRefreshToken", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }

                var successMessage = _localizationService.GetMessage("TokenRefreshed", "Messages", language);
                return Ok(BaseResponse<LoginResponse>.SuccessResponse(response, successMessage));
            }
        }

        /// <summary>
        /// التحقق من صحة الرمز
        /// </summary>
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] ValidateTokenRequest request)
        {
            // استخراج اللغة المفضلة من رأس الطلب
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);
            {
                var user = _jwtService.ValidateJwtToken(request.Token);
                if (user == null)
                {
                    var errorMessage = _localizationService.GetMessage("InvalidToken", "Errors", language);
                    return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
                }
            }

            var successMessage = _localizationService.GetMessage("TokenValid", "Messages", language);
            return Ok(BaseResponse.SuccessResponse(successMessage));
        }
    }
}