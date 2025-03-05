namespace MauiKit.Views.Dashboards;

public partial class DashboardCarouselPage : BasePage
{
	public DashboardCarouselPage()
	{
		InitializeComponent();
        dashboardTiles.SelectionChanged += OnSelectionChanged;
    }
    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as NavigationMenuItem;
        if (item != null)
        {
            Application.Current.MainPage.DisplayAlert("Item Tapped!", $"You have selected: {item.Title}", "OK");
        }
    }
}

