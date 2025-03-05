namespace MauiKit.Views.News;

public partial class  NewsProfilePage : BasePage
{
	public NewsProfilePage()
	{
		InitializeComponent();
		BindingContext = new NewsProfileViewModel();
	}
}
