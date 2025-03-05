namespace MauiKit.Views.Properties;

public partial class PropertyListingClasicPage : BasePage
{

    public PropertyListingClasicPage()
    {
        InitializeComponent();
        BindingContext = new PropertyListingClasicViewModel();
    }

    private void CollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView) collectionView.SelectedItem = null;
    }

}

