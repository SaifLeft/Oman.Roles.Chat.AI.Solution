using System.Diagnostics;

namespace Maui.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlert("خطأ", "الرجاء إدخال اسم المستخدم وكلمة المرور", "موافق");
            return;
        }

        LoadingIndicator.IsRunning = true;
        LoginButton.IsEnabled = false;

        try
        {
            var success = await _viewModel.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);

            if (success)
            {
                // Navigate to main page after successful login
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("خطأ", "اسم المستخدم أو كلمة المرور غير صحيحة", "موافق");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login error: {ex.Message}");
            await DisplayAlert("خطأ", "حدث خطأ أثناء تسجيل الدخول، يرجى المحاولة مرة أخرى", "موافق");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoginButton.IsEnabled = true;
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }

    private async void OnPhoneLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("PhoneLoginPage");
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsRunning = true;
        GoogleLoginButton.IsEnabled = false;

        try
        {
            var success = await _viewModel.LoginWithGoogleAsync();

            if (success)
            {
                // Navigate to main page after successful login
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("خطأ", "فشل تسجيل الدخول باستخدام حساب Google", "موافق");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Google login error: {ex.Message}");
            await DisplayAlert("خطأ", "حدث خطأ أثناء تسجيل الدخول باستخدام Google", "موافق");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            GoogleLoginButton.IsEnabled = true;
        }
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ForgotPasswordPage");
    }
}