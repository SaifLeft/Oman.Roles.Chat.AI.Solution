namespace MauiKit.Views;
public partial class TransferSuccessPopup : PopupPage
{
	public TransferSuccessPopup()
	{
		InitializeComponent();
	}

    private async void ViewReceiptButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}