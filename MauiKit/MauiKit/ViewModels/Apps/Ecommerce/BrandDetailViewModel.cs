
namespace MauiKit.ViewModels.Ecommerce;
public class BrandDetailViewModel : BaseViewModel
{
    public ICommand TapCommand { get; private set; }
    public ICommand TapCommandMenu { get; private set; }

    public ObservableCollection<TabPageModel> _TabPageList = new ObservableCollection<TabPageModel>();
    public ObservableCollection<TabPageModel> TabPageList
    {
        get
        {
            return _TabPageList;
        }
        set
        {
            _TabPageList = value;
            OnPropertyChanged("TabPageList");
        }
    }

    public ObservableCollection<ProductDetail> _AllProductDataList = new ObservableCollection<ProductDetail>();
    public ObservableCollection<ProductDetail> AllProductDataList
    {
        get
        {
            return _AllProductDataList;
        }
        set
        {
            _AllProductDataList = value;
            OnPropertyChanged("AllProductDataList");
        }
    }
    public BrandDetailViewModel()
    {
        PopulateData();
        TapCommand = new Command<ProductDetail>(SelectProduct);
        TapCommandMenu = new Command<TabPageModel>(SelectMenu);
    }
    private async void SelectProduct(ProductDetail obj)
    {
        await Application.Current.MainPage.Navigation.PushModalAsync(new ProductDetailPage());
    }

    private void SelectMenu(TabPageModel obj)
    {
        foreach (var item in TabPageList)
        {
            if (item.Id == obj.Id)
            {
                item.IsSelected = true;
            }
            else
            {
                item.IsSelected = false;
            }
        }

    }
    void PopulateData()
    {
        AllProductDataList.Clear();
        AllProductDataList = new ObservableCollection<ProductDetail>(EcommerceServices.Instance.GetAllProducts);

        TabPageList.Clear();
        TabPageList.Add(new TabPageModel("All", 0, true));
        TabPageList.Add(new TabPageModel("Smart Bluetooth Speaker", 1, false));
        TabPageList.Add(new TabPageModel("Lamp", 2, false));
        TabPageList.Add(new TabPageModel("Airpods", 3, false));
    }
}
