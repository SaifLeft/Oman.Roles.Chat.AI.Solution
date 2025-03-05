namespace MauiKit.Views;

public partial class PolarChartsPage : BasePage
{
	public PolarChartsPage()
	{
		InitializeComponent();
		BindingContext = new PolarChartsViewModel();
    }
}

