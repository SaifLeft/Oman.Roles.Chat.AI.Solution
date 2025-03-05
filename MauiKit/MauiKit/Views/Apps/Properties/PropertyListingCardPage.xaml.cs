namespace MauiKit.Views.Properties;

public partial class PropertyListingCardPage : BasePage
{

    public PropertyListingCardPage()
    {
        InitializeComponent();
        BindingContext = new PropertyListingCardViewModel();
    }

    private void CollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView) collectionView.SelectedItem = null;
    }

    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

