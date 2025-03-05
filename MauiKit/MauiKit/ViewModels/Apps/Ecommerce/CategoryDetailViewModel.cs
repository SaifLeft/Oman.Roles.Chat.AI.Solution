
namespace MauiKit.ViewModels.Ecommerce;
public partial class CategoryDetailViewModel : BaseViewModel
{
    public CategoryDetailViewModel()
    {
        LoadData();
    }

    [ObservableProperty]
    private List<ProductDetail> _productsList;

    [ObservableProperty]
    private List<BrandListModel> _brandsList;

    void LoadData()
    {
        BrandsList = new List<BrandListModel>(EcommerceServices.Instance.GetFeaturedBrands);
        ProductsList = new List<ProductDetail>(EcommerceServices.Instance.GetAllProducts);
    }

}
