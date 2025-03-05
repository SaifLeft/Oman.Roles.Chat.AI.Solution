namespace MauiKit.Views.Forms;

public partial class FullBackgroundSignUpPage : ContentPage
{
	public FullBackgroundSignUpPage()
	{
		InitializeComponent();
	}

    private void SignupButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}