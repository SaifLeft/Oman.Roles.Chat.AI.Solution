namespace MauiKit.Views;
public partial class ListImageRoundedPage : BasePage
{
	public ListImageRoundedPage()
	{
		InitializeComponent();
		BindingContext = new ListImageRoundedViewModel();
	}
}