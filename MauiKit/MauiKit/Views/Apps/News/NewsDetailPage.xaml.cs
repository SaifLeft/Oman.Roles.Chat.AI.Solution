namespace MauiKit.Views.News;

public partial class NewsDetailPage : BasePage
{
	public NewsDetailPage()
	{
		InitializeComponent();

        BindingContext = new NewsDetailViewModel();
    }
}
