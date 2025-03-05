namespace MauiKit.Views.News;

public partial class BookmarksPage : BasePage
{
	public BookmarksPage()
	{
		InitializeComponent();
		this.BindingContext = new BookmarksViewModel();
	}
}
