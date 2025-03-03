using System.Diagnostics;

namespace Maui.Service
{
    public interface IAuthService
    {
        Task<string> GetTokenAsync();
        Task SaveTokenAsync(string token);
        Task<string> GetRefreshTokenAsync();
        Task SaveRefreshTokenAsync(string refreshToken);
        Task<bool> IsAuthenticatedAsync();
        Task<string> GetIPAddressAsync();
        Task<string> GetUserAgentAsync();
        Task<GoogleAuthResult> AuthenticateWithGoogleAsync();
        Task LogoutAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IPreferencesService _preferencesService;

        private const string TokenKey = "AuthToken";
        private const string RefreshTokenKey = "RefreshToken";
        private const string UserAgentString = "SmartLawyer-MobileApp/1.0";

        public AuthService(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        public async Task<string> GetTokenAsync()
        {
            return await _preferencesService.GetValueAsync(TokenKey);
        }

        public async Task SaveTokenAsync(string token)
        {
            await _preferencesService.SaveValueAsync(TokenKey, token);
        }

        public async Task<string> GetRefreshTokenAsync()
        {
            return await _preferencesService.GetValueAsync(RefreshTokenKey);
        }

        public async Task SaveRefreshTokenAsync(string refreshToken)
        {
            await _preferencesService.SaveValueAsync(RefreshTokenKey, refreshToken);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string> GetIPAddressAsync()
        {
            // In a real app, you might use a service to get the actual IP address
            return "127.0.0.1";
        }

        public Task<string> GetUserAgentAsync()
        {
            return Task.FromResult(UserAgentString);
        }

        public async Task<GoogleAuthResult> AuthenticateWithGoogleAsync()
        {
            try
            {
                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri("https://accounts.google.com/o/oauth2/auth" +
                           "?client_id=YOUR_CLIENT_ID" +
                           "&redirect_uri=YOUR_REDIRECT_URI" +
                           "&response_type=code" +
                           "&scope=openid%20profile%20email"),
                    new Uri("YOUR_REDIRECT_URI"));

                var accessToken = authResult?.AccessToken;

                if (string.IsNullOrEmpty(accessToken))
                {
                    return new GoogleAuthResult
                    {
                        IsAuthenticated = false
                    };
                }

                // In a real app, you would validate this token with your server
                // For now, we'll just extract basic info from the token

                var userId = authResult.Properties.ContainsKey("sub") ?
                    authResult.Properties["sub"] : "unknown";

                var email = authResult.Properties.ContainsKey("email") ?
                    authResult.Properties["email"] : "unknown@example.com";

                return new GoogleAuthResult
                {
                    IsAuthenticated = true,
                    Token = accessToken,
                    UserId = userId,
                    Email = email
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Google Authentication Error: {ex.Message}");
                return new GoogleAuthResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task LogoutAsync()
        {
            await _preferencesService.RemoveValueAsync(TokenKey);
            await _preferencesService.RemoveValueAsync(RefreshTokenKey);
            await _preferencesService.SaveValueAsync("IsAuthenticated", "false");
        }
    }

    public class GoogleAuthResult
    {
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
    }
}