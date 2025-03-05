namespace MauiKit.Views.Travels;

public partial class TravelAccountPage : BasePage
{
	public TravelAccountPage()
	{
		InitializeComponent();
        BindingContext = new TravelAccountViewModel();
    }

    private void CollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView) collectionView.SelectedItem = null;
    }
}