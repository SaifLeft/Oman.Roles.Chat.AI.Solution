namespace MauiKit.Views;

public partial class PropertyIndexPage : BasePage
{
	public PropertyIndexPage()
	{
		InitializeComponent();
	}

    async void HomePage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PropertyHomePage());
    }

    async void ListingCard_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PropertyListingCardPage());
    }

    async void ListingClasic_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PropertyListingClasicPage());
    }

    async void MapView_Tapped(object sender, EventArgs e)
    {
#if WINDOWS
        Application.Current.MainPage.DisplayAlert("Sorry!", "Maps is not supported.", "OK");
#else
        await Navigation.PushAsync(new PropertyMapPage());
#endif
    }

    async void Booking_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PropertyBookingPage());
    }

    async void Payment_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BookingPaymentPage());
    }

    async void PropertyDetail_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new PropertyDetailPage());
    }

    async void AgentProfile_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgentProfilePage());
    }

    async void Settings_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PropertySettingsPage());
        //await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}