namespace MauiKit.Views;
public partial class CardOptionsPopup : PopupPage
{
	public CardOptionsPopup()
	{
		InitializeComponent();
	}

    private async void OnChangeLimit_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
    private async void OnTopup_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
    private async void OnRefund_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
    private async void OnTemporaryBlock_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
    private async void OnPermanentBlock_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}