namespace MauiKit.Views.Dashboards.Templates;

public partial class DashboardNotificationItemTemplate : DashboardBaseItemTemplate
{
	public DashboardNotificationItemTemplate()
	{
		InitializeComponent();
	}

    protected override void OnTapped()
    {
        Application.Current.MainPage.DisplayAlert("Tile Tapped!", "You have tapped a DashboardVariantItemTemplate", "OK");
    }
}