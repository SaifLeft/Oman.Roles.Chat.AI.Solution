
namespace MauiKit.ViewModels.Ecommerce;
public class CartViewModel : BaseViewModel
{
    public ICommand TapCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

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

    public CartViewModel()
    {
        PopulateData();
        CommandInit();
    }

    void PopulateData()
    {
        AllProductDataList.Clear();
        AllProductDataList = new ObservableCollection<ProductDetail>(EcommerceServices.Instance.GetAllProducts);
    }

    private void CommandInit()
    {
        TapCommand = new Command<ProductListModel>(items =>
        {
            Application.Current.MainPage.Navigation.PushAsync(new ProductDetailPage());
        });

        DeleteCommand = new Command<ProductListModel>(items =>
        {

        });


    }
}
