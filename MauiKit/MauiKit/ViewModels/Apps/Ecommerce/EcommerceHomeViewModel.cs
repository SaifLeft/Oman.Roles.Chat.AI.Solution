namespace MauiKit.ViewModels.Ecommerce;
public partial class EcommerceHomeViewModel : BaseViewModel
{
    [ObservableProperty]
    private List<BannerModel> _homeBanners;

    [ObservableProperty]
    private List<CategoryModel> _categoriesList;

    [ObservableProperty]
    private List<ProductListModel> _bestSellingList;

    [ObservableProperty]
    private List<ProductListModel> _recommendedList;

    [ObservableProperty]
    private List<BrandListModel> _featuredBrandsList;

    public EcommerceHomeViewModel()
    {
        LoadData();
    }

    private void LoadData()
    {
        HomeBanners = new List<BannerModel>(EcommerceServices.Instance.GetBanners);
        CategoriesList = new List<CategoryModel>(EcommerceServices.Instance.GetCategories);
        BestSellingList = new List<ProductListModel>(EcommerceServices.Instance.GetBestSellers);
        RecommendedList = new List<ProductListModel>(EcommerceServices.Instance.GetRecommendeds);
        FeaturedBrandsList = new List<BrandListModel>(EcommerceServices.Instance.GetFeaturedBrands);
    }
}
