using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs.Authorization;
using System.Text.Json;

namespace Services
{
    public interface IGoogleAuthService
    {
        /// <summary>
        /// الحصول على رابط تسجيل الدخول باستخدام Google
        /// </summary>
        /// <param name="redirectUri">عنوان إعادة التوجيه بعد المصادقة</param>
        /// <returns>رابط تسجيل الدخول</returns>
        string GetLoginUrl(string redirectUri);

        /// <summary>
        /// معالجة رمز المصادقة من Google والحصول على معلومات المستخدم
        /// </summary>
        /// <param name="code">رمز المصادقة</param>
        /// <param name="redirectUri">عنوان إعادة التوجيه</param>
        /// <returns>معلومات المستخدم</returns>
        Task<UserDTO> AuthenticateAsync(string code, string redirectUri);
    }

    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAuthService> _logger;
        private readonly IJwtService _jwtService;

        public GoogleAuthService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GoogleAuthService> logger,
            IJwtService jwtService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _jwtService = jwtService;
        }

        /// <summary>
        /// الحصول على رابط تسجيل الدخول باستخدام Google
        /// </summary>
        public string GetLoginUrl(string redirectUri)
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("Google Client ID is not configured");
            }

            var scope = "email profile";
            var responseType = "code";
            var accessType = "offline";
            var prompt = "consent";

            var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                      $"?client_id={clientId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&response_type={responseType}" +
                      $"&scope={Uri.EscapeDataString(scope)}" +
                      $"&access_type={accessType}" +
                      $"&prompt={prompt}";

            return url;
        }

        /// <summary>
        /// معالجة رمز المصادقة من Google والحصول على معلومات المستخدم
        /// </summary>
        public async Task<UserDTO> AuthenticateAsync(string code, string redirectUri)
        {
            // التحقق من وجود رمز المصادقة
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Authentication code is required", nameof(code));
            }

            // الحصول على إعدادات المصادقة
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("Google authentication configuration is missing");
            }

            // إرسال طلب للحصول على رمز الوصول
            var tokenRequestParameters = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            };

            var tokenRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
            var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequestContent);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();

            var tokenInfo = JsonSerializer.Deserialize<GoogleTokenResponse>(tokenResponseJson);

            if (tokenInfo == null || string.IsNullOrEmpty(tokenInfo.AccessToken))
            {
                throw new Exception("Failed to get access token from Google");
            }

            // استخدام رمز الوصول للحصول على معلومات المستخدم
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenInfo.AccessToken}");

            var userInfoResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            userInfoResponse.EnsureSuccessStatusCode();

            var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
            var googleUserInfo = JsonSerializer.Deserialize<GoogleUserInfo>(userInfoJson);

            if (googleUserInfo == null || string.IsNullOrEmpty(googleUserInfo.Id))
            {
                throw new Exception("Failed to get user information from Google");
            }

            // تحويل معلومات المستخدم من Google إلى نموذج المستخدم الخاص بنا
            var userInfo = new UserDTO
            {
                Id = googleUserInfo.Id,
                Username = googleUserInfo.Email,
                Email = googleUserInfo.Email,
                FirstName = googleUserInfo.Name,
                LastName = googleUserInfo.FamilyName,
                Roles = new List<string> { "User" } // منح دور المستخدم العادي بشكل افتراضي
            };

            return userInfo;
        }
    }
}