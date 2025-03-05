
namespace MauiKit.Views.Travels;

public partial class TravelSettingsPage : BasePage
{
    TravelSettingsViewModel viewModel;
    public TravelSettingsPage()
    {
        InitializeComponent();
        BindingContext = viewModel = new TravelSettingsViewModel(Navigation, this);
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void Logout_Tapped(object sender, TappedEventArgs e)
    {

    }
}