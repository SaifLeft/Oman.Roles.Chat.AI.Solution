
namespace MauiKit.Views.Actions;

public partial class ShippingInfoPopupPage : PopupPage
{
	public ShippingInfoPopupPage()
	{
		InitializeComponent();
	}

    private async void SubmitButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}