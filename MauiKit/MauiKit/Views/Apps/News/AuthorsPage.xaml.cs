namespace MauiKit.Views.News;

public partial class AuthorsPage : BasePage
{
	public AuthorsPage()
	{
		InitializeComponent();
		BindingContext = new AuthorsViewModel();
	}

    private void FollowBtn_Clicked(object sender, EventArgs e)
    {

    }

    private void OnDarkMode_Tapped(object sender, TappedEventArgs e)
    {

    }
}