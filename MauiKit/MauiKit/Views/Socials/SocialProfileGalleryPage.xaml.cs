namespace MauiKit.Views.Socials;

public partial class SocialProfileGalleryPage : ContentPage
{
	public SocialProfileGalleryPage()
	{
		InitializeComponent();
		BindingContext = new SocialProfileGalleryViewModel();
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