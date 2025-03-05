
namespace MauiKit.Views.Actions;

public partial class PaymentMethodPopupPage : PopupPage
{
	public PaymentMethodPopupPage()
	{
		InitializeComponent();
	}

    private async void OKButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}