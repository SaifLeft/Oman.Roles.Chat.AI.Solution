namespace MauiKit.Views.Travels;

public partial class TravelMessageDetailPage : BasePage
{
    TravelMessageDetailViewModel viewModel;
    public TravelMessageDetailPage(TravelMessage selectedConversation)
    {
        InitializeComponent();
        BindingContext = viewModel = new TravelMessageDetailViewModel(selectedConversation);

        NavigationPage.SetHasNavigationBar(this, false);
    }

}