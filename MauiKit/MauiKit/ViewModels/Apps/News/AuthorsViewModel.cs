
namespace MauiKit.ViewModels.News;
public partial class AuthorsViewModel : ObservableObject
{
    public AuthorsViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        TopAuthors = new List<Author>(MockNewsServices.Instance.GetAllAuthors.Where(x => x.IsFeatured == true));
        AllAuthors = new List<Author>(MockNewsServices.Instance.GetAllAuthors);
    }

    #region Public Properties

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    List<Author> topAuthors;

    [ObservableProperty]
    List<Author> allAuthors;

    #endregion Public Properties
}
