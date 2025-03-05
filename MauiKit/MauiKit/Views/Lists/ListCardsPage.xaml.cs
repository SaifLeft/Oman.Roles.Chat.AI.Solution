namespace MauiKit.Views;
public partial class ListCardsPage : BasePage
{
	public ListCardsPage()
	{
		InitializeComponent();
		BindingContext = new ListCardsViewModel();
	}
}