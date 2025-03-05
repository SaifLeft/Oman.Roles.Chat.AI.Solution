namespace MauiKit.Views.Travels;

public partial class TravelMessagesPage : BasePage
{
    TravelMessagesViewModel viewModel;
    public TravelMessagesPage()
    {
        InitializeComponent();
        BindingContext = viewModel = new TravelMessagesViewModel(Navigation, this);

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnCloseButtonClicked(object sender, EventArgs args)
    {
        await Navigation.PopAsync();
    }
}