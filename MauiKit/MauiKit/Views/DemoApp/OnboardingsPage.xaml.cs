namespace MauiKit.Views;

public partial class OnboardingsPage : BasePage
{
	public OnboardingsPage()
	{
		InitializeComponent();
	}

    async void WelcomePage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StartPage());
        //await Navigation.PushAsync(new WalkthroughPage());
    }
    async void WelcomeVariantPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StartVariantPage());
    }
    async void WelcomeBackgroundPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StartBackgroundPage());
    }
    async void WalkthroughAnimation_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughAnimationPage());
    }
    async void WalkthroughGradient_Tapped(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new WalkthroughGradientPage());
    }
    async void WalkthroughIllustration_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughIllustrationPage());
    }
    async void WalkthroughImage1_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughImage1Page());
    }
    async void WalkthroughImage2_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughImage2Page());
    }
    async void WalkthroughStyle1_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughStyle1Page());
    }
    async void WalkthroughStyle2_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughStyle2Page());
    }
}