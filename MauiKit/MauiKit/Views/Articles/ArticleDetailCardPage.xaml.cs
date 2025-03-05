namespace MauiKit.Views.Articles;

public partial class ArticleDetailCardPage : BasePage
{
	public ArticleDetailCardPage()
	{
		InitializeComponent();
		BindingContext = new ArticleDetailCardViewModel();
    }

    private void OnAddComment_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Button Tapped", "Add Comment", "OK");
    }
}