
namespace MauiKit.ViewModels.Travels;
public partial class TravelAccountViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TravelArticle> _myArticles;

    [ObservableProperty]
    private TravelUser _user;

    public TravelAccountViewModel()
    {
        LoadData();
    }

    #region Methods
    void LoadData()
    {
        User = TravelGuideServices.Instance.GetUsers().ToList().FirstOrDefault();
        MyArticles = new ObservableCollection<TravelArticle>(TravelGuideServices.Instance.PopularTravelGuides);
    }
    #endregion Methods
}
