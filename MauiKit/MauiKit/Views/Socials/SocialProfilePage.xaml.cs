namespace MauiKit.Views.Socials;

public partial class SocialProfilePage : ContentPage
{
	public SocialProfilePage()
	{
		InitializeComponent();
		BindingContext = new SocialProfileViewModel();
	}

    private async void OnImagePreviewDoubleTapped(object sender, EventArgs args)
    {
        const uint AnimationDuration = 100;

        if ((int)img.Scale == 1)
        {
            await img.ScaleTo(2, AnimationDuration, Easing.SinInOut);
        }
        else
        {
            await img.ScaleTo(1, AnimationDuration, Easing.SinInOut);
        }
    }

    private async void OnCloseButtonTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}