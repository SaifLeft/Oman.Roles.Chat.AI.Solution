namespace MauiKit.Views.Forms;

public partial class FullBackgroundLoginPage : ContentPage
{
	public FullBackgroundLoginPage()
	{
		try
        {
            InitializeComponent();
        }
        catch(Exception e)
        {

        }
	}

    private void LoginButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }

    private async void ForgotPassword_Tapped(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new ForgotPasswordPage());
    }

    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}