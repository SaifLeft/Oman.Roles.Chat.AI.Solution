
namespace MauiKit.ViewModels.News;
public partial class VideoNewsViewModel : ObservableObject
{
    public VideoNewsViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        Categories = new ObservableCollection<NewsCategory>(MockNewsServices.Instance.GetCategories);
        ArticleLists = new ObservableCollection<ArticleData>(ArticleServices.Instance.GetArticles());
    }

    #region Public Properties

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    public ObservableCollection<NewsCategory> _categories;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _articleLists;

    #endregion Public Properties
}
