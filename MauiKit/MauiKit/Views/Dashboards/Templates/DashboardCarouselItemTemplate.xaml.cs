namespace MauiKit.Views.Dashboards.Templates;

public partial class DashboardCarouselItemTemplate : DashboardBaseItemTemplate
{
	public DashboardCarouselItemTemplate()
	{
		InitializeComponent();
	}

    protected override void OnTapped()
    {
        Application.Current.MainPage.DisplayAlert("Tile Tapped!", "You have tapped a Grid Item Template", "OK");
    }
}