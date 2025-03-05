namespace MauiKit.Views.Articles;

public partial class AddArticlePage : BasePage
{
	public AddArticlePage()
	{
		InitializeComponent();
        BindingContext = new AddArticleViewModel();
    }
}