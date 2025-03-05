
namespace MauiKit.Views.News;

public partial class VideoNewsPage : BasePage
{
	public VideoNewsPage()
	{
		InitializeComponent();
		BindingContext = new VideoNewsViewModel();
	}
}