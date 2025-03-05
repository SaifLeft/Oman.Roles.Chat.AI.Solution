namespace MauiKit.Views.Forms;

public partial class PasswordVerificationPage : BasePage
{
	public PasswordVerificationPage()
	{
		InitializeComponent();
	}

    private async void Verification_Click(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
}