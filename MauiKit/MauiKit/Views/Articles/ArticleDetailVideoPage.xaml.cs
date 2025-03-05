namespace MauiKit.Views.Articles;

public partial class ArticleDetailVideoPage : BasePage
{
	public ArticleDetailVideoPage()
	{
		InitializeComponent();
		BindingContext = new ArticleDetailVideoViewModel();
    }

    private void OnAddComment_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Button Tapped", "Add Comment", "OK");
    }

    private async void OnPlayButton_Clicked(object sender, EventArgs e)
    {
        ArticleData defaultArticle = ArticleServices.Instance.GetArticle(001);
        await Navigation.PushAsync(new VideoPlayerPage(defaultArticle));
    }
}