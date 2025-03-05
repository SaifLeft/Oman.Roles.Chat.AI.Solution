
using MauiKit.ViewModels.Ecommerce;
using static MauiKit.Models.Ecommerce.TrackOrderModel;

namespace MauiKit.Views;

public partial class EcommerceIndexPage : BasePage
{
    public EcommerceIndexPage()
	{
		InitializeComponent();
	}

	async void Dashboard_Tapped(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new EcommerceHomePage());
    }

    async void Category_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CategoryDetailPage());
    }

    async void ProductsGrid_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductCatalogGridPage());
    }

    async void ProductLists_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductCatalogListPage());
    }

    async void ProductDetails_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductDetailPage());
    }
    async void OrderConfirmation_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrderConfirmationPage());
    }

    async void Checkout_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CheckoutPage());
    }

    async void OrdersHistory_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrderHistoryPage());
    }

    async void OrderTracking_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TrackOrderPage());
    }

    async void Account_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EcommerceProfilePage());
    }
}