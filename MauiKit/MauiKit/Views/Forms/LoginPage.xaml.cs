namespace MauiKit.Views.Forms;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private void LoginButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}