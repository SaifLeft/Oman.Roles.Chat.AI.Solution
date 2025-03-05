namespace MauiKit.Views.Dashboards.Templates;

public partial class DashboardGridItemTemplate : DashboardBaseItemTemplate
{
	public DashboardGridItemTemplate()
	{
		InitializeComponent();
	}

    protected override void OnTapped()
    {
        Application.Current.MainPage.DisplayAlert("Tile Tapped!", string.Format(LocalizationResourceManager.Translate("StringGridItemTappedMessage")), "OK");
    }
}