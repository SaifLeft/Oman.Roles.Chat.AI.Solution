
namespace MauiKit.ViewModels.Travels;
public partial class TravelHomeViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<PlaceCategory> _placeCategories;

    [ObservableProperty]
    private ObservableCollection<TravelArticle> _popularTravelGuides;

    [ObservableProperty]
    private ObservableCollection<TravelArticle> _followingAuthorGuides;

    #region Constructor
    public TravelHomeViewModel()
    {
        LoadData();
    }
    #endregion Constructor

    #region Methods
    void LoadData()
    {
        PlaceCategories = new ObservableCollection<PlaceCategory>(TravelGuideServices.Instance.PlaceCategories);
        PopularTravelGuides = new ObservableCollection<TravelArticle>(TravelGuideServices.Instance.PopularTravelGuides);
        FollowingAuthorGuides = new ObservableCollection<TravelArticle>(TravelGuideServices.Instance.PopularTravelGuides);
    }
    #endregion Methods

}
