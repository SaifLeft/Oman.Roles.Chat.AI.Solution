namespace MauiKit.Views.News;

public partial class NewsHomePage : BasePage
{
    NewsHomeViewModel viewModel;
    public NewsHomePage()
	{
		InitializeComponent();
		BindingContext = viewModel = new NewsHomeViewModel(Navigation, this);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
}
