namespace MauiKit.Views.Dashboards;
public partial class DashboardEventPage : BasePage
{
	public DashboardEventPage()
	{
		InitializeComponent();
		BindingContext = new DashboardEventViewModel();
	}
}