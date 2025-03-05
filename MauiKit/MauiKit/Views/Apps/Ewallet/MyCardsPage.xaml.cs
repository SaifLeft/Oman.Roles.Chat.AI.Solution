namespace MauiKit.Views.Ewallet;

public partial class MyCardsPage : BasePage
{
	public MyCardsPage()
	{
		InitializeComponent();
        BindingContext = new MyCardsViewModel();
    }
}