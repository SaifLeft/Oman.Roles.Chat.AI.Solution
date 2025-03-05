namespace MauiKit.Views.Dashboards.Templates;

public partial class DashboardGridInverseItemTemplate : DashboardBaseItemTemplate
{
	public DashboardGridInverseItemTemplate()
	{
		InitializeComponent();
	}

    protected override void OnTapped()
    {
        Application.Current.MainPage.DisplayAlert("Tile Tapped!", string.Format(LocalizationResourceManager.Translate("StringGridItemTappedMessage")), "OK");
    }
}