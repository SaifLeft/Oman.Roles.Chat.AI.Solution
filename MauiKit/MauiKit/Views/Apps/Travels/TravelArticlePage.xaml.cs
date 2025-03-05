
namespace MauiKit.Views.Travels;
public partial class TravelArticlePage : BasePage
{
	public TravelArticlePage()
	{
		InitializeComponent();
        BindingContext = new TravelArticleViewModel();
    }

    private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        if (e.ScrollY > 10)
        {
            navBarStack.BackgroundColor = Color.FromRgba("#FFFFFF");
            backButton.BackgroundColor = ResourceHelper.FindResource<Color>("T_Primary");
            backButton.Stroke = ResourceHelper.FindResource<Color>("T_Primary");
            favButton.BackgroundColor = ResourceHelper.FindResource<Color>("T_Primary");
            favButton.Stroke = ResourceHelper.FindResource<Color>("T_Primary");
        }
        else
        {
            navBarStack.BackgroundColor = Color.FromRgba("#00000000");
            backButton.BackgroundColor = Color.FromRgba("#10000000");
            backButton.Stroke = ResourceHelper.FindResource<Color>("White");
            favButton.BackgroundColor = Color.FromRgba("#10000000");
            favButton.Stroke = ResourceHelper.FindResource<Color>("White");
        }
    }

    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
