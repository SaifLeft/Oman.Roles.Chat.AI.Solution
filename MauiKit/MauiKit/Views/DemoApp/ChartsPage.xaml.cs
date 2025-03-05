namespace MauiKit.Views;

public partial class ChartsPage : BasePage
{
	public ChartsPage()
	{
		InitializeComponent();
	}

    private async void CartesianChart_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new CartesianChartsPage());
    }
    private async void PieChart_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PieChartsPage());
    }
    private async void PolarChart_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PolarChartsPage());
    }
    private async void GeoMap_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new GeoMapPage());
    }
}