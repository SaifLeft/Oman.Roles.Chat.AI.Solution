namespace MauiKit.Views;

public partial class AboutPage : BasePage
{
	public AboutPage()
	{
		InitializeComponent();
		BindingContext = new AboutViewModel();
	}
}