namespace MauiKit.Views.Travels;

public partial class TravelHomePage : BasePage
{
    public TravelHomePage()
    {
        InitializeComponent();
        BindingContext = new TravelHomeViewModel();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
    private void CollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView) collectionView.SelectedItem = null;
    }
}

