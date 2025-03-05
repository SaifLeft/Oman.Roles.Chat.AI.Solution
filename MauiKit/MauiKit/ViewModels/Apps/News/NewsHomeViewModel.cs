namespace MauiKit.ViewModels.News;
public partial class NewsHomeViewModel : ObservableObject
{
    private INavigation _navigationService;
    private Page _pageService;

    public NewsHomeViewModel(INavigation navigationService, Page pageService)
		{
        _navigationService = navigationService;
        _pageService = pageService;

        LoadData();
    }

    public void LoadData()
    {
        TrendingCategories = new ObservableCollection<TrendingCategory>(MockNewsServices.Instance.GetTrendingCategories);
        Tags = new ObservableCollection<string>(MockNewsServices.Instance.GetTags);
        LatestArticles = new ObservableCollection<Article>(MockNewsServices.Instance.GetLatestArticles);
        RecentArticles = new ObservableCollection<Article>(MockNewsServices.Instance.GetRecentArticles);
        TopChannels = new ObservableCollection<Channel>(MockNewsServices.Instance.GetTopChannels);
    }

    #region Public Properties

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    public ObservableCollection<TrendingCategory> _trendingCategories;

    [ObservableProperty]
    public ObservableCollection<string> _tags;

    [ObservableProperty]
    public ObservableCollection<Article> _latestArticles;

    [ObservableProperty]
    public ObservableCollection<Article> _recentArticles;

    [ObservableProperty]
    public ObservableCollection<Channel> _topChannels;

    #endregion Public Properties
    
}
