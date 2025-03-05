namespace MauiKit.Views;

public partial class NewsIndexPage : BasePage
{
	public NewsIndexPage()
	{
		InitializeComponent();
	}

    async void HomePage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewsHomePage());
    }

    async void CategoriesPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CategoriesPage());
    }

    async void VideosPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VideoNewsPage());
    }

    async void BookmarksPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BookmarksPage());
    }

    async void ArticleDetailPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewsDetailPage());
    }

    async void AuthorsPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AuthorsPage());
    }

    async void AccountPage_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewsProfilePage());
    }
}