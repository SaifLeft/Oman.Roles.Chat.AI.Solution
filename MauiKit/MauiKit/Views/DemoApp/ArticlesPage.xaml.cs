namespace MauiKit.Views;

public partial class ArticlesPage : BasePage
{
	public ArticlesPage()
	{
		InitializeComponent();
	}

    async void AddArticle_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddArticlePage());
    }

    async void ArticleParalaxHeader_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticleParallaxHeaderPage());
    }

    async void ArticleCurvedMask_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticleCurvedMaskPage());
    }

    async void ArticleDetailCard_Tapped(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new ArticleDetailCardPage());
    }

    async void ArticleDetailVideo_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticleDetailVideoPage());
    }

    async void ArticlesClassic_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticlesClassicPage());
    }

    async void ArticlesCardOverlay_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticlesCardOverlayPage());
    }

    async void ArticlesVideo_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticlesVideoPage());
    }

    async void VideoDetail_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VideoDetailPage());
    }
}