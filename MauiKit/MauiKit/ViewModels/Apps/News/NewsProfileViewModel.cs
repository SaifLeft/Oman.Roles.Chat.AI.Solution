
namespace MauiKit.ViewModels.News;
public partial class NewsProfileViewModel : ObservableObject
{
    public NewsProfileViewModel()
    {
        LoadData();
    }

    #region Public Properties
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    ObservableCollection<Article> _latestArticles;

    #endregion Public Properties

    public void LoadData()
    {
        LatestArticles = new ObservableCollection<Article>(MockNewsServices.Instance.GetLatestArticles);
    }
}
