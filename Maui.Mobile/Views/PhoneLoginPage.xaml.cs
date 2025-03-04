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
            await DisplayAlert("���", "������ ����� ��� ������ ����� ������", "�����");
            return;
        }

        // Check if phone number is valid
        if (!long.TryParse(PhoneEntry.Text, out long phoneNumber))
        {
            await DisplayAlert("���", "������ ����� ��� ���� ����", "�����");
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
                await DisplayAlert("���", "��� ������ �� ���� ������ ��� �����", "�����");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Phone login error: {ex.Message}");
            await DisplayAlert("���", "��� ��� ����� ����� ������ ���� �������� ��� ����", "�����");
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