namespace MauiKit.Views.Forms;

public partial class BackgroundGradientSignUpPage : ContentPage
{
	public BackgroundGradientSignUpPage()
	{
		InitializeComponent();
	}

    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void SignupButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
}