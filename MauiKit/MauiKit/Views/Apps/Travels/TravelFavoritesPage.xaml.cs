namespace MauiKit.Views.Travels;

public partial class TravelFavoritesPage : BasePage
{
    TravelFavoritesViewModel viewModel;
    public TravelFavoritesPage()
	{
		InitializeComponent();
        BindingContext = viewModel = new TravelFavoritesViewModel(Navigation, this);
    }
}