
namespace MauiKit.Views.Onboardings;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
	}

    private async void TakeTour_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughImage2Page());
    }

    private async void Skip_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}