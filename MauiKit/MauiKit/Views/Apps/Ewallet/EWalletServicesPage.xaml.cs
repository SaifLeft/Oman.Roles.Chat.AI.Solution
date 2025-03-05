namespace MauiKit.Views.Ewallet;
public partial class EwalletServicesPage : BasePage
{
	public EwalletServicesPage()
	{
		InitializeComponent();
		BindingContext = new EwalletServicesViewModel();
    }
}