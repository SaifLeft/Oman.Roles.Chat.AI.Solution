using API.Client;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Maui.Mobile.Views;

public partial class ForgotPasswordPage : ContentPage
{
    private readonly IAPIClient _apiClient;

    public ForgotPasswordPage(IAPIClient apiClient)
    {
        InitializeComponent();
        _apiClient = apiClient;
    }


    private async void OnSendResetCodeClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EmailEntry.Text))
        {
            await DisplayAlert("Œÿ√", "«·—Ã«¡ ≈œŒ«· «·»—Ìœ «·≈·ﬂ —Ê‰Ì", "„Ê«›ﬁ");
            return;
        }

        // Validate email format
        if (!IsValidEmail(EmailEntry.Text))
        {
            await DisplayAlert("Œÿ√", "«·—Ã«¡ ≈œŒ«· »—Ìœ ≈·ﬂ —Ê‰Ì ’ÕÌÕ", "„Ê«›ﬁ");
            return;
        }

        LoadingIndicator.IsRunning = true;
        SendButton.IsEnabled = false;

        try
        {
            // Here you would call your service to send a reset code
            // For demo purposes, we'll just show a success message
            await Task.Delay(2000); // Simulate network delay

            await DisplayAlert("‰Ã«Õ", " „ ≈—”«· —„“ ≈⁄«œ…  ⁄ÌÌ‰ ﬂ·„… «·„—Ê— ≈·Ï »—Ìœﬂ «·≈·ﬂ —Ê‰Ì", "„Ê«›ﬁ");

            // Show reset code entry fields
            ResetCodeSection.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Send reset code error: {ex.Message}");
            await DisplayAlert("Œÿ√", "ÕœÀ Œÿ√ √À‰«¡ ≈—”«· —„“ ≈⁄«œ… «· ⁄ÌÌ‰° Ì—ÃÏ «·„Õ«Ê·… „—… √Œ—Ï", "„Ê«›ﬁ");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            SendButton.IsEnabled = true;
        }
    }


    private bool IsValidEmail(string text)
    {
        var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        return emailRegex.IsMatch(text);
    }
}