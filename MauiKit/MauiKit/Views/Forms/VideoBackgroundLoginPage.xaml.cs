namespace MauiKit.Views.Forms;

public partial class VideoBackgroundLoginPage : ContentPage
{
	public VideoBackgroundLoginPage()
	{
		try
        {
            InitializeComponent();
        }
        catch(Exception e)
        {

        }
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        MediaPlayer.Stop();
        base.OnDisappearing();
    }

    private void LoginButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
    private async void ForgotPassword_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage());
    }

    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void PageUnloaded(object sender, EventArgs e)
    {
        MediaPlayer.Handler?.DisconnectHandler();
    }
}