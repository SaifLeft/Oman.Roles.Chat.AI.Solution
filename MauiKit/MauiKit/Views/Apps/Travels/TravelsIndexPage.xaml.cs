namespace MauiKit.Views;

public partial class TravelsIndexPage : BasePage
{
	public TravelsIndexPage()
	{
		InitializeComponent();
        BindingContext = this;
	}

    async void HomePage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TravelHomePage());
    }

    async void ExplorePage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TravelExplorePage());
    }

    async void ArticleDetailPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TravelArticlePage());
    }

    async void RecentMessagesPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TravelMessagesPage());
    }

    async void MessageDetailPage_Tapped(object sender, EventArgs e)
    {
        TravelMessage defaultConversation = TravelGuideServices.Instance.GetRecentMessages.FirstOrDefault();
        await Navigation.PushAsync(new TravelMessageDetailPage(defaultConversation));
    }

    async void AccountPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TravelAccountPage());
    }

    async void UpdateProfilePage_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ProfileUpdatePage());
    }

    async void SettingsPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TravelSettingsPage());
    }
}