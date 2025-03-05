namespace MauiKit.Views;

public partial class DashboardsPage : BasePage
{
	public DashboardsPage()
	{
		InitializeComponent();
	}

    private async void DashboardTasks_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardTasksPage());
    }
    private async void DashboardCoverFlow_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardCoverFlowPage());
    }
    private async void DashboardArticle_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardArticlePage());
    }

    private async void DashboardCard_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardCardPage());
    }
    private async void DashboardEvent_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardEventPage());
    }
    private async void DashboardTimeline_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardTimelinePage());
    }
    private async void DashboardVisual_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardVisualPage());
    }
    private async void DashboardGrid_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardGridPage());
    }
    private async void DashboardGridInverse_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardGridInversePage());
    }
    private async void DashboardCarousel_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DashboardCarouselPage());
    }

    async void DashboardVariant_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashboardVariantsPage());
    }
    async void DefaultTabbedPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DefaultTabbedPage());
    }
    async void AndroidBottomTabbedPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AndroidBottomTabbedPage());
    }
}