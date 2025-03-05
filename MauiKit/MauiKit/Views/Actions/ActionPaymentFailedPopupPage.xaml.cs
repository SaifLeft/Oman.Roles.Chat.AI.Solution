namespace MauiKit.Views.Actions;

public partial class ActionPaymentFailedPopupPage : PopupPage
{
	public ActionPaymentFailedPopupPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}