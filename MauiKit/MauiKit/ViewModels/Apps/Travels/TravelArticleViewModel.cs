
namespace MauiKit.ViewModels.Travels;
public partial class TravelArticleViewModel : BaseViewModel
{
    [ObservableProperty]
    private TravelArticle _articleDetail;

    [ObservableProperty]
    private ObservableCollection<TravelArticle> _relatedGuides;

    public TravelArticleViewModel()
	{
        InitData();
    }

    public void InitData()
    {
        ArticleDetail = TravelGuideServices.Instance.PopularTravelGuides.FirstOrDefault();
        RelatedGuides = new ObservableCollection<TravelArticle>(TravelGuideServices.Instance.PopularTravelGuides);
    }
}
