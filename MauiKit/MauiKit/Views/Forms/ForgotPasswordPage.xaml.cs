namespace MauiKit.Views.Forms;

public partial class ForgotPasswordPage : BasePage
{
	public ForgotPasswordPage()
	{
		InitializeComponent();
	}

    private async void ResetPassword_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
}