
namespace MauiKit.Views.Articles;

public partial class VideoPlayerPage : BasePage
{
    public VideoPlayerPage(ArticleData articleData)
	{
		InitializeComponent();
        BindingContext = new VideoPlayerViewModel(articleData);

#if ANDROID
        mediaPlayer.VerticalOptions = LayoutOptions.Center;
#endif
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        
    }
    protected override void OnDisappearing()
    {
        mediaPlayer.Stop();
        base.OnDisappearing();
    }
}