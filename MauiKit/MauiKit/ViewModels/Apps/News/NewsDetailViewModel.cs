
namespace MauiKit.ViewModels.News;
public partial class NewsDetailViewModel : ObservableObject
{
    public NewsDetailViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        ArticleDetail = MockNewsServices.Instance.GetArticleDetail;
        RelatedNews = new ObservableCollection<Article>(MockNewsServices.Instance.GetLatestArticles);
    }

    #region Public Properties
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    Article _articleDetail;

    [ObservableProperty]
    ObservableCollection<Article> _relatedNews;
    #endregion Public Properties

}
