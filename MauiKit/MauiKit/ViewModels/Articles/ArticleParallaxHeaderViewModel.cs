
namespace MauiKit.ViewModels.Articles;
public partial class ArticleParallaxHeaderViewModel : ObservableObject
{
    public ArticleParallaxHeaderViewModel() 
    {
        LoadData();
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
                Article = ArticleServices.Instance.GetArticle(003);
                RelatedArticles = new ObservableCollection<ArticleData>(ArticleServices.Instance.GetArticles());
                Gallery = new ObservableCollection<ArticleData>()
                {
                    new ArticleData
                    {
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/blog-1.jpg",
                    },
                    new ArticleData
                    {
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/blog-2.jpg",
                    },
                    new ArticleData
                    {
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/blog-3.jpg",
                    },
                    new ArticleData
                    {
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/blog-4.jpg",
                    },
                };
                IsBusy = false;
            });
        });
    }

    #endregion Methods

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ArticleData _article;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _relatedArticles;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _gallery;
}
