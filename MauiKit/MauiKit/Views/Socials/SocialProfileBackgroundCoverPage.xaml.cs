namespace MauiKit.Views.Socials;

public partial class SocialProfileBackgroundCoverPage : ContentPage
{
	public SocialProfileBackgroundCoverPage()
	{
		InitializeComponent();
		BindingContext = new SocialProfileBackgroundCoverViewModel();
	}

    private async void OnEdit(object sender, EventArgs e)
    {
        await DisplayAlert("Edit tapped", "Navigate to the edit contact page.", "OK");
    }
    private async void OnCloseButtonTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}