
namespace MauiKit.Views.Properties;

public partial class PropertyHomePage : BasePage
{
    public PropertyHomePage()
    {
        InitializeComponent();
        BindingContext = new PropertyHomeViewModel();
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

