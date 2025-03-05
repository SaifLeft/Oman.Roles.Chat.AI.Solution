namespace MauiKit.Views.Ewallet;

public partial class PaymentConfirmPage : BasePage
{
	public PaymentConfirmPage()
	{
		InitializeComponent();
        BindingContext = new PaymentConfirmViewModel();
    }
}