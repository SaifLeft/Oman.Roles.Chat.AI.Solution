namespace MauiKit.Views;

public partial class EwalletIndexPage : BasePage
{
	public EwalletIndexPage()
	{
		InitializeComponent();
	}

    async void HomePage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EwalletHomePage());
    }

    async void ServicesPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EwalletServicesPage());
    }

    async void TopupPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MobileTopupPage());
    }

    async void BillPaymentPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BillPaymentPage());
    }

    async void QrScanPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ScanQrPayPage());
    }

    async void PaymentConfirmPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PaymentConfirmPage());
    }

    async void EReceiptPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EReceiptPage());
    }

    async void TransferPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TransferMoneyPage());
    }

    async void StatisticsPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StatisticsPage());
    }

    async void MyCardPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MyCardsPage());
    }
}