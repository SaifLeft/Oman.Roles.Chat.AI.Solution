using Maui.Mobile.ViewModels;
using System.Diagnostics;

namespace Maui.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // Validate input fields
        if (string.IsNullOrEmpty(UsernameEntry.Text) ||
            string.IsNullOrEmpty(FullNameEntry.Text) ||
            string.IsNullOrEmpty(EmailEntry.Text) ||
            string.IsNullOrEmpty(PhoneEntry.Text) ||
            string.IsNullOrEmpty(PasswordEntry.Text) ||
            string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("Œÿ√", "Ì—ÃÏ  ⁄»∆… Ã„Ì⁄ «·ÕﬁÊ· «·„ÿ·Ê»…", "„Ê«›ﬁ");
            return;
        }

        // Validate email format
        if (!IsValidEmail(EmailEntry.Text))
        {
            await DisplayAlert("Œÿ√", "Ì—ÃÏ ≈œŒ«· »—Ìœ ≈·ﬂ —Ê‰Ì ’ÕÌÕ", "„Ê«›ﬁ");
            return;
        }

        // Validate phone number
        if (!IsValidPhoneNumber(PhoneEntry.Text))
        {
            await DisplayAlert("Œÿ√", "Ì—ÃÏ ≈œŒ«· —ﬁ„ Â« › ’ÕÌÕ", "„Ê«›ﬁ");
            return;
        }

        // Validate password
        if (PasswordEntry.Text.Length < 6)
        {
            await DisplayAlert("Œÿ√", "ÌÃ» √‰  ﬂÊ‰ ﬂ·„… «·„—Ê— 6 √Õ—› ⁄·Ï «·√ﬁ·", "„Ê«›ﬁ");
            return;
        }

        // Validate password match
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Œÿ√", "ﬂ·„… «·„—Ê— Ê √ﬂÌœ ﬂ·„… «·„—Ê— €Ì— „ ÿ«»ﬁ«‰", "„Ê«›ﬁ");
            return;
        }

        LoadingIndicator.IsRunning = true;
        RegisterButton.IsEnabled = false;

        try
        {
            long phoneNumber = 0;
            if (long.TryParse(PhoneEntry.Text, out phoneNumber))
            {
                var success = await _viewModel.RegisterAsync(
                    UsernameEntry.Text,
                    EmailEntry.Text,
                    PasswordEntry.Text,
                    FullNameEntry.Text,
                    phoneNumber);

                if (success)
                {
                    await DisplayAlert("‰Ã«Õ", " „  ”ÃÌ· Õ”«»ﬂ »‰Ã«Õ° Ì„ﬂ‰ﬂ «·¬‰  ”ÃÌ· «·œŒÊ·", "„Ê«›ﬁ");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlert("Œÿ√", "›‘· ›Ì  ”ÃÌ· «·Õ”«»° ﬁœ ÌﬂÊ‰ «”„ «·„” Œœ„ √Ê «·»—Ìœ «·≈·ﬂ —Ê‰Ì „” Œœ„ „”»ﬁ«", "„Ê«›ﬁ");
                }
            }
            else
            {
                await DisplayAlert("Œÿ√", "Ì—ÃÏ ≈œŒ«· —ﬁ„ Â« › ’ÕÌÕ", "„Ê«›ﬁ");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Register error: {ex.Message}");
            await DisplayAlert("Œÿ√", "ÕœÀ Œÿ√ √À‰«¡ ≈‰‘«¡ «·Õ”«»° Ì—ÃÏ «·„Õ«Ê·… „—… √Œ—Ï", "„Ê«›ﬁ");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            RegisterButton.IsEnabled = true;
        }
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Simple validation for now - check if it's a number and has at least 8 digits
        return !string.IsNullOrEmpty(phoneNumber) &&
               phoneNumber.All(char.IsDigit) &&
               phoneNumber.Length >= 8;
    }
}