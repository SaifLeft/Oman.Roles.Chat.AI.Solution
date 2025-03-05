namespace MauiKit.Views.Ewallet;

public partial class MobileTopupPage : BasePage
{
	public MobileTopupPage()
	{
		InitializeComponent();
		BindingContext = new MobileTopupViewModel();

    }
}