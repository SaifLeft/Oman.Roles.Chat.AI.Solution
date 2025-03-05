
namespace MauiKit.Views.Onboardings;

public partial class StartVariantPage : ContentPage
{
	public StartVariantPage()
	{
		InitializeComponent();
	}

    private async void TakeTour_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WalkthroughStyle1Page());
    }

    private async void Skip_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}