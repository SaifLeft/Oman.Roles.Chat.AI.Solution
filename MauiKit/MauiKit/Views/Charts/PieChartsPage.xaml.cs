namespace MauiKit.Views;

public partial class PieChartsPage : BasePage
{
	public PieChartsPage()
	{
		InitializeComponent();
		BindingContext = new PieChartsViewModel();
    }
}

