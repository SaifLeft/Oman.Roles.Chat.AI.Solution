namespace MauiKit.Views.Articles;

public partial class ArticlesVideoPage : BasePage
{
	public ArticlesVideoPage()
	{
		InitializeComponent();
		BindingContext = new ArticlesVideoViewModel(Navigation);
    }
}