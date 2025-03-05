
namespace MauiKit.ViewModels.Travels;
public partial class TravelExploreViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TravelArticle> _travelArticles;

    #region Constructor
    public TravelExploreViewModel()
    {
        LoadData();
    }
    #endregion Constructor

    #region Methods
    void LoadData()
    {
        TravelArticles = new ObservableCollection<TravelArticle>(TravelGuideServices.Instance.PopularTravelGuides);
    }
    #endregion Methods
}
