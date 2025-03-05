
namespace MauiKit.Views.Properties;
public partial class PropertyMapPage : BasePage
{
	public PropertyMapPage()
	{
		InitializeComponent();
        BindingContext = new PropertyMapViewModel();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var hanaLoc = new Location(20.7557, -155.9880);

        MapSpan mapSpan = MapSpan.FromCenterAndRadius(hanaLoc, Distance.FromKilometers(3));
        map.MoveToRegion(mapSpan);

        map.Pins.Add(new Pin
        {
            Label = "Subscribe to our channel?",
            Location = new Location(50.8514, 5.6910),
        });

        map.Pins.Add(new Pin
        {
            Label = "Welcome to MAUIKIT!",
            Location = hanaLoc,
        });
    }

    private void CollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView) collectionView.SelectedItem = null;
    }

    private void searchLocation_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        searchResult.IsVisible = true;
    }
}