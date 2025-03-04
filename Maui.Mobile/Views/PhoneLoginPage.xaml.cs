using Maui.Mobile.ViewModels;
using System.Diagnostics;

namespace Maui.Mobile.Views;

public partial class PhoneLoginPage : ContentPage
{
    private readonly PhoneLoginViewModel _viewModel;

    public PhoneLoginPage(PhoneLoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(PhoneEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlert("Œÿ√", "«·—Ã«¡ ≈œŒ«· —ﬁ„ «·Â« › Êﬂ·„… «·„—Ê—", "„Ê«›ﬁ");
            return;
        }

        // Check if phone number is valid
        if (!long.TryParse(PhoneEntry.Text, out long phoneNumber))
        {
            await DisplayAlert("Œÿ√", "«·—Ã«¡ ≈œŒ«· —ﬁ„ Â« › ’ÕÌÕ", "„Ê«›ﬁ");
            return;
        }

        LoadingIndicator.IsRunning = true;
        LoginButton.IsEnabled = false;

        try
        {
            var success = await _viewModel.LoginWithPhoneAsync(phoneNumber, PasswordEntry.Text);

            if (success)
            {
                // Navigate to main page after successful login
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("Œÿ√", "—ﬁ„ «·Â« › √Ê ﬂ·„… «·„—Ê— €Ì— ’ÕÌÕ…", "„Ê«›ﬁ");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Phone login error: {ex.Message}");
            await DisplayAlert("Œÿ√", "ÕœÀ Œÿ√ √À‰«¡  ”ÃÌ· «·œŒÊ·° Ì—ÃÏ «·„Õ«Ê·… „—… √Œ—Ï", "„Ê«›ﬁ");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoginButton.IsEnabled = true;
        }
    }

    private async void OnBackToLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ForgotPasswordPage");
    }
}