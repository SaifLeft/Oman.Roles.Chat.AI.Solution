namespace MauiKit.Views;
public partial class ListIconPage : BasePage
{
	public ListIconPage()
	{
		InitializeComponent();
		BindingContext = new ListIconViewModel();
	}
}