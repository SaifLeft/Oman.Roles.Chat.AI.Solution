
namespace MauiKit.ViewModels.Travels;
public partial class TravelFavoritesViewModel : BaseViewModel
{
    #region Fields
    private INavigation _navigationService;
    private Page _pageService;

    [ObservableProperty]
    private ObservableCollection<TravelArticle> _travelArticles;
    #endregion Fields

    #region Constructor
    public TravelFavoritesViewModel(INavigation navigationService, Page pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;

        LoadData();

        ArticleDetailCommand = new Command<TravelArticle>(OpenArticleDetail);
    }
    #endregion Constructor

    #region Commands
    public ICommand ArticleDetailCommand { get; private set; }
    private async void OpenArticleDetail(TravelArticle selectedArticle)
    {
        await _navigationService.PushModalAsync(new TravelArticlePage());
    }

    #endregion Commands

    #region Methods
    void LoadData()
    {
        TravelArticles = new ObservableCollection<TravelArticle>(TravelGuideServices.Instance.PopularTravelGuides);
    }
    #endregion Methods

}
