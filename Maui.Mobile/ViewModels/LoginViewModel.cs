using API.Client;
using Maui.Service;
using System.Diagnostics;
using System.Windows.Input;

namespace Maui.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAPIClient _apiClient;
        private readonly IAuthService _authService;
        private readonly IPreferencesService _preferencesService;

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoogleLoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginViewModel(
            IAPIClient apiClient,
        IAuthService authService,
            IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _authService = authService;
            _preferencesService = preferencesService;

            Title = "تسجيل الدخول";

            LoginCommand = new Command<(string, string)>((params) => LoginAsync(Item1, Item2));
            GoogleLoginCommand = new Command(async () => await LoginWithGoogleAsync());
            RegisterCommand = new Command(async () => await GoToRegisterPage());
            ForgotPasswordCommand = new Command(async () => await GoToForgotPasswordPage());
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            IsBusy = true;

            try
            {
                var loginRequest = new LoginRequestDTO
                {
                    Username = username,
                    Password = password,
                    IpAddress = await _authService.GetIPAddressAsync(),
                    UserAgent = await _authService.GetUserAgentAsync()
                };

                var response = await _apiClient.LoginAsync(loginRequest);
                //var responseContent = await GetResponseContent(response);

                //if (responseContent == null)
                //    return false;

                var loginResponse = response.Data;

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.AccessToken))
                    return false;

                // Save the token and user info
                await _authService.SaveTokenAsync(loginResponse.AccessToken);
                await _authService.SaveRefreshTokenAsync(loginResponse.RefreshToken);
                await _preferencesService.SaveValueAsync("UserId", loginResponse.User.Id.ToString());
                await _preferencesService.SaveValueAsync("Username", loginResponse.User.Username);
                await _preferencesService.SaveValueAsync("IsAuthenticated", "true");

                return true;
            }
            catch (ApiException ex)
            {
                Debug.WriteLine($"API Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login Error: {ex.Message}");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            IsBusy = true;

            try
            {
                var googleAuthResult = await _authService.AuthenticateWithGoogleAsync();

                if (!googleAuthResult.IsAuthenticated)
                    return false;

                // TODO: Implement server-side validation of Google token
                // For now, we'll consider it successful if we get a token from Google

                // Save user info
                await _preferencesService.SaveValueAsync("UserId", googleAuthResult.UserId);
                await _preferencesService.SaveValueAsync("Username", googleAuthResult.Email);
                await _preferencesService.SaveValueAsync("IsAuthenticated", "true");
                await _authService.SaveTokenAsync(googleAuthResult.Token);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Google Login Error: {ex.Message}");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<string> GetResponseContent(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task GoToRegisterPage()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }

        private async Task GoToForgotPasswordPage()
        {
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
    }
}