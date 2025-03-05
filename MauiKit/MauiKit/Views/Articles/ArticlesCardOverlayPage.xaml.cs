namespace MauiKit.Views.Articles;

public partial class ArticlesCardOverlayPage : BasePage
{
	public ArticlesCardOverlayPage()
	{
		InitializeComponent();
		BindingContext = new ArticlesCardOverlayViewModel();
    }

    private async void OnItemTapped(object sender, EventArgs e)
    {
#if !NAVIGATION
        var selectedItem = ((ListView)sender).SelectedItem;
        //var articlePage = new ArticleDetailsPage(selectedItem as ArticleData);

        //await Navigation.PushAsync(articlePage);
#endif
    }
}