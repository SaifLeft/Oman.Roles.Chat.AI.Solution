namespace MauiKit.Views.Ewallet;

public partial class TransferMoneyPage : BasePage
{
	public TransferMoneyPage()
	{
		InitializeComponent();
        BindingContext = new TransferMoneyViewModel();
    }
}