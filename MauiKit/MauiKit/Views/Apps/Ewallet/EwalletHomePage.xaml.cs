namespace MauiKit.Views.Ewallet;

public partial class EwalletHomePage : BasePage
{
	public EwalletHomePage()
	{
		InitializeComponent();
        BindingContext = new EwalletHomeViewModel();
    }
}