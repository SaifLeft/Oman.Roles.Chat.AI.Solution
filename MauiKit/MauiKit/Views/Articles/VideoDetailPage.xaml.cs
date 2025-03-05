namespace MauiKit.Views.Articles;

public partial class VideoDetailPage : BasePage
{
    public VideoDetailPage()
	{
        InitializeComponent();

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