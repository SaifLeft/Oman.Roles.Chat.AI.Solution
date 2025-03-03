using API.Client;
using Maui.Service;
using System.Windows.Input;

namespace Maui.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAPIClient _apiClient;
        private readonly IAuthService _authService;

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
            private async Task GoToLoginPage()
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    private string _email;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _confirmPassword;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    private string _fullName;
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    private string _phoneNumber;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    private bool _acceptTerms;
    public bool AcceptTerms
    {
        get => _acceptTerms;
        set => SetProperty(ref _acceptTerms, value);
    }

    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    public RegisterViewModel(
        IAPIClient apiClient,
        IAuthService authService)
    {
        _apiClient = apiClient;
        _authService = authService;

        Title = "إنشاء حساب جديد";

        RegisterCommand = new Command(async () =>
        {
            long phoneNum = 0;
            if (long.TryParse(PhoneNumber, out phoneNum))
            {
                await RegisterAsync(Username, Email, Password, FullName, phoneNum);
            }
        });

        GoToLoginCommand = new Command(async () => await GoToLoginPage());

        // Default value
        AcceptTerms = false;
    }
}

