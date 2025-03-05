namespace MauiKit.Views.Articles;

public partial class ArticlesClassicPage : BasePage
{
    ArticlesClassicViewModel viewModel;
    public ArticlesClassicPage()
	{
		InitializeComponent();
		BindingContext = viewModel = new ArticlesClassicViewModel();
    }

    private async void OnItemTapped(object sender, EventArgs e)
    {
#if !NAVIGATION
        var selectedItem = ((ListView)sender).SelectedItem;
        //var articlePage = new ArticleDetailsPage(selectedItem as ArticleData);
        //await Navigation.PushAsync(articlePage);
        viewModel.IsSelected = true;
        await Application.Current.MainPage.DisplayAlert("Item Tapped!", $"You have tapped an article item", "OK");
#endif
    }
}