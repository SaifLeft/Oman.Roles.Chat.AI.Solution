using MauiKit.Services;
using MauiKit.Views.News;
using MauiKit.Views.Properties;
using MauiKit.Views;
using System.Xml.Linq;

namespace MauiKit.Views;

public partial class AppIndexPage : BasePage
{
    public AppIndexPage()
	{
		InitializeComponent();
        BindingContext = new AppIndexViewModel();
    }

	async void Ewallet_Tapped(object sender, EventArgs e)
	{
        //await Application.Current.MainPage.DisplayAlert(string.Format(LocalizationResourceManager.Translate("StringCommingSoon")), string.Format(LocalizationResourceManager.Translate("StringMessageFeatureCommingSoon")), "OK");
        await Navigation.PushAsync(new EwalletIndexPage());
    }

    async void Travel_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TravelsIndexPage());
    }
    async void RealEstate_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PropertyIndexPage());
    }
    async void Ecommerce_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EcommerceIndexPage());
    }

    async void News_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewsIndexPage());
    }

}