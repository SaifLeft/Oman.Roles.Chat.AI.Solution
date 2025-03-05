
namespace MauiKit.Views.Onboardings;

public partial class StartBackgroundPage : ContentPage
{
	public StartBackgroundPage()
	{
		InitializeComponent();
	}

    private async void TakeTour_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughImage1Page());
    }

    private async void Skip_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}