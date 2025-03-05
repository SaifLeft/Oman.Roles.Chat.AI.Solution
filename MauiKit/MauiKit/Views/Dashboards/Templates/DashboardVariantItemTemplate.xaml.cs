namespace MauiKit.Views.Dashboards.Templates;

public partial class DashboardVariantItemTemplate : DashboardBaseItemTemplate
{
	public DashboardVariantItemTemplate()
	{
		InitializeComponent();
	}

    protected override void OnTapped()
    {
        Application.Current.MainPage.DisplayAlert("Tile Tapped!", "You have tapped a Variant Item Template", "OK");
    }
}