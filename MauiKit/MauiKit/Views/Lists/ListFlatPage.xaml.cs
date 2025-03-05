namespace MauiKit.Views;
public partial class ListFlatPage : BasePage
{
	public ListFlatPage()
	{
		InitializeComponent();
		BindingContext = new ListFlatViewModel();
	}
}