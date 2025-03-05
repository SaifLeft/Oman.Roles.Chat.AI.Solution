
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiKit.ViewModels.Ecommerce
{
    public partial class CheckoutViewModel : ObservableObject
    {
        public ICommand TapCommand { get; private set; }
        public Command<object> RecommendedTapCommand { get; private set; }


        #region Properties

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private double _total;

        [ObservableProperty]
        private List<ProductListModel> _relatedProducts = new List<ProductListModel>();

        [ObservableProperty]
        private List<ProductListModel> _selectedProducts = new List<ProductListModel>();

        [ObservableProperty]
        private List<CartItemModel> _carts = new List<CartItemModel>();

        #endregion Properties
        public CheckoutViewModel()
        {
            InitData();

            TapCommand = new Command<ProductListModel>(SelectProduct);
            RecommendedTapCommand = new Command<object>(SelectRecommend);
            
        }
        void InitData()
        {
            Carts.Clear();
            Carts.Add(new CartItemModel
            {
                ProductName = "Boulder Boot",
                BrandName = "MAUIKIT",
                ProductImage = AppSettings.ImageServerPath +  "ecommerce/01.png",
                Price = 39.90,
                Quantity = 3,
            });
            Carts.Add(new CartItemModel
            {
                ProductName = "College Bag",
                BrandName = "MAUIKIT",
                ProductImage = AppSettings.ImageServerPath +  "ecommerce/02.png",
                Price = 49.90,
                Quantity = 2,
            });

            Total = Carts.Sum(x => x.Amount);

            RelatedProducts.Clear();
            RelatedProducts.Add(new ProductListModel() { Name = "Flannel Shirt", BrandName = "MAUIKIT", Price = "$39.90", ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_0.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_1.jpg" } });
            RelatedProducts.Add(new ProductListModel() { Name = "Bomber Jacket", BrandName = "MAUIKIT", Price = "$89.90", ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_1.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_1.jpg" } });
            RelatedProducts.Add(new ProductListModel() { Name = "Classic Black", BrandName = "MAUIKIT", Price = "$49.90", ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_2.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_1.jpg" } });
            RelatedProducts.Add(new ProductListModel() { Name = "Flowers Shirt", BrandName = "MAUIKIT", Price = "$29.90", ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_3.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_1.jpg" } });
        }

        public void OnOrderQuantityChanged(int quantity)
        {
            Total = Carts.Sum(x => x.Amount);
        }

        private async void SelectProduct(ProductListModel obj)
        {
            //await Application.Current.MainPage.Navigation.PushModalAsync(new ProductDetails());
        }

        private async void SelectRecommend(object obj)
        {
            //await Application.Current.MainPage.Navigation.PushAsync(new AllProduct());
        }
    }
}
