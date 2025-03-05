namespace MauiKit.Views.Travels;

public partial class TravelExplorePage : BasePage
{
	public TravelExplorePage()
	{
		InitializeComponent();
        BindingContext = new TravelExploreViewModel();
    }

    private async void BackButton_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}