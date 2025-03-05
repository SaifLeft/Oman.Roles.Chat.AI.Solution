namespace MauiKit.Views.Articles;

public partial class ArticleParallaxHeaderPage : BasePage
{
	public ArticleParallaxHeaderPage()
	{
		InitializeComponent();
		BindingContext = new ArticleParallaxHeaderViewModel();

    }

    private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        if(e.ScrollY > 10)
        {
            navBarStack.BackgroundColor = Color.FromRgba("#FFFFFFAF");
            navBackIcon.TextColor = Color.FromRgba("#313131");
            navPageTitle.TextColor = Color.FromRgba("#313131");
        }
        else
        {
            navBarStack.BackgroundColor = Color.FromRgba("#00000000");
            navBackIcon.TextColor = Color.FromRgba("#FFFFFF");
            navPageTitle.TextColor = Color.FromRgba("#FFFFFF");
        }
    }

    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnAddComment_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Button Tapped", "Add Comment", "OK");
    }
}