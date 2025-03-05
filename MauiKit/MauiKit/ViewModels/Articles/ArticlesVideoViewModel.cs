
namespace MauiKit.ViewModels.Articles;
public partial class ArticlesVideoViewModel : ObservableObject
{
    private INavigation _navigationService;
    public ArticlesVideoViewModel(INavigation navigationService) 
    {
        _navigationService = navigationService;

        LoadData();

        ItemTappedCommand = new Command<ArticleData>(ArticleDetailCommand);
    }

    #region Methods
    void LoadData()
    {
        IsBusy = true;
        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                ArticleLists = new ObservableCollection<ArticleData>(ArticleServices.Instance.GetArticles());
                IsBusy = false;
            });
        });
    }

    #endregion Methods

    #region Commands

    public ICommand ItemTappedCommand { get; private set; }

    async void ArticleDetailCommand(ArticleData article)
    {
        if (article == null)
            return;
        await _navigationService.PushAsync(new VideoPlayerPage(article));
    }

    #endregion Commands


    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _articleLists;
}
