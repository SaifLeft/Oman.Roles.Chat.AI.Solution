namespace MauiKit.Views.Socials;

public partial class SocialProfileCardPage : ContentPage
{
	public SocialProfileCardPage()
	{
		InitializeComponent();
		BindingContext = new SocialProfileCardViewModel();
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