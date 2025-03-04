using API.Client;
using System.Diagnostics;

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
            await DisplayAlert("���", "������ ����� ������ ����������", "�����");
            return;
        }

        // Validate email format
        if (!IsValidEmail(EmailEntry.Text))
        {
            await DisplayAlert("���", "������ ����� ���� �������� ����", "�����");
            return;
        }

        LoadingIndicator.IsRunning = true;
        SendButton.IsEnabled = false;

        try
        {
            // Here you would call your service to send a reset code
            // For demo purposes, we'll just show a success message
            await Task.Delay(2000); // Simulate network delay

            await DisplayAlert("����", "�� ����� ��� ����� ����� ���� ������ ��� ����� ����������", "�����");

            // Show reset code entry fields
            ResetCodeSection.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Send reset code error: {ex.Message}");
            await DisplayAlert("���", "��� ��� ����� ����� ��� ����� ������� ���� �������� ��� ����", "�����");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            SendButton.IsEnabled = true;
        }
    }

    private async void OnResetPasswordClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ResetCodeEntry.Text) ||
            string.IsNullOrEmpty(NewPasswordEntry.Text) ||
            string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("���", "������ ����� ���� ������ ��������", "�����");
            return;
        }

        // Validate password
        if (NewPasswordEntry.Text.Length < 6)
        {
            await DisplayAlert("���", "��� �� ���� ���� ������ 6 ���� ��� �����", "�����");
            return;
        }

        // Validate password match
        if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("���", "���� ������ ������ ���� ������ ��� ��������", "�����");
            return;
        }

        ResetLoadingIndicator.IsRunning = true;
        ResetButton.IsEnabled = false;

        try
        {
            // Create reset password request
            var resetRequest = new ResetPasswordRequestDTO
            {
                Email = EmailEntry.Text,
                ResetCode = ResetCodeEntry.Text,
                NewPassword = NewPasswordEntry.Text,
                ConfirmPassword = ConfirmPasswordEntry.Text,
                IpAddress = "127.0.0.1", // In a real app, get this dynamically
                UserAgent = "SmartLawyer-MobileApp/1.0" // In a real app, get this dynamically
            };

            // Call API to reset password
            await _apiClient.ResetPasswordAsync(resetRequest);

            await DisplayAlert("����", "�� ����� ����� ���� ������ ����͡ ����� ���� ����� ������ �������� ���� ������ �������", "�����");

            // Navigate back to login page
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Reset password error: {ex.Message}");
            await DisplayAlert("���", "��� ��� ����� ����� ����� ���� �����ѡ ���� ������ �� ��� ��� ������ ��������� ��� ����", "�����");
        }
        finally
        {
            ResetLoadingIndicator.IsRunning = false;
            ResetButton.IsEnabled = true;
        }
    }

    private async void OnBackToLoginTapped(object sender, EventArgs e)
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
}