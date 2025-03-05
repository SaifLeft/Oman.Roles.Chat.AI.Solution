namespace MauiKit.Views;

public partial class CartesianChartsPage : BasePage
{
	public CartesianChartsPage()
	{
		InitializeComponent();
		BindingContext = new CartesianChartsViewModel();
    }
}

