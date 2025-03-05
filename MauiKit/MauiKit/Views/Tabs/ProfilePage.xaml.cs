namespace MauiKit.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel();
	}

    private async void OnEdit(object sender, EventArgs e)
    {
        await DisplayAlert("Edit tapped", "Navigate to the edit contact page.", "OK");
    }
}