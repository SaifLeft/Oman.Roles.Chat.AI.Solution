namespace MauiKit.Views;

public partial class GeoMapPage : BasePage
{
	public GeoMapPage()
	{
		InitializeComponent();
		BindingContext = new GeoMapViewModel();
    }
}

