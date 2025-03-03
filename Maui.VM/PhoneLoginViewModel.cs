using API.Client;
using Maui.Service;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Input;

namespace Maui.ViewModels
{
    public class PhoneLoginViewModel : BaseViewModel
    {
        private readonly IAPIClient _apiClient;
        private readonly IAuthService _authService;
        private readonly IPreferencesService _preferencesService;

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginWithPhoneCommand { get; }
        public ICommand BackToLoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public PhoneLoginViewModel(
            IAPIClient apiClient,
            IAuthService authService,
            IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _authService = authService;
            _preferencesService = preferencesService;

            Title = "تسجيل الدخول برقم الهاتف";

            LoginWithPhoneCommand = new Command<(long, string)>(async (params) =>
                await LoginWithPhoneAsync(params.Item1, params.Item2));

            BackToLoginCommand = new Command(async () => await GoToLoginPage());
            ForgotPasswordCommand = new Command(async () => await GoToForgotPasswordPage());
        }

        public async Task<bool> LoginWithPhoneAsync(long phoneNumber, string password)
        {
            if (phoneNumber <= 0 || string.IsNullOrEmpty(password))
                return false;

            IsBusy = true;

            try
            {
                var loginRequest = new LoginWithPhoneRequestDTO
                {
                    PhoneNumber = phoneNumber,
                    Password = password,
                    IpAddress = await _authService.GetIPAddressAsync(),
                    UserAgent = await _authService.GetUserAgentAsync()
                };

                var response = await _apiClient.LoginWithPhoneAsync(loginRequest);
                var responseContent = await GetResponseContent(response);

                if (responseContent == null)
                    return false;

                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseContent);

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                    return false;

                // Save the token and user info
                await _authService.SaveTokenAsync(loginResponse.Token);
                await _authService.SaveRefreshTokenAsync(loginResponse.RefreshToken);
                await _preferencesService.SaveValueAsync("UserId", loginResponse.UserId.ToString());
                await _preferencesService.SaveValueAsync("Username", loginResponse.Username);
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
                Debug.WriteLine($"Phone Login Error: {ex.Message}");
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

        private async Task GoToLoginPage()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task GoToForgotPasswordPage()
        {
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }
    }
}