namespace MauiKit.Views;
public partial class ListImagePage : BasePage
{
	public ListImagePage()
	{
		InitializeComponent();
		BindingContext = new ListImageViewModel();
	}
}