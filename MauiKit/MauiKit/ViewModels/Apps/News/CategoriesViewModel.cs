
namespace MauiKit.ViewModels.News;
public partial class CategoriesViewModel : ObservableObject
{
    public CategoriesViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        Sections = new ObservableCollection<NewsCategory>(MockNewsServices.Instance.GetCategories);
    }

    #region Public Properties
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    public ObservableCollection<NewsCategory> _sections;

    #endregion Public Properties
}

