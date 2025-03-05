
namespace MauiKit.ViewModels.News;
public partial class BookmarksViewModel : ObservableObject
{
    public BookmarksViewModel()
	{
        LoadData();
    }

    public void LoadData()
    {
        Articles = new ObservableCollection<Article>(MockNewsServices.Instance.GetBookmarkedArticles);
    }

    #region Public Properties
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    public ObservableCollection<Article> _articles;

    #endregion Public Properties

}
