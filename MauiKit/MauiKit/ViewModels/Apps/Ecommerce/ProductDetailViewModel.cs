
using System.Windows.Input;

namespace MauiKit.ViewModels.Ecommerce
{
    public class ProductDetailViewModel : BaseViewModel
    {
        double lastScrollIndex;
        double currentScrollIndex;
        public ICommand TapBackCommand { get; set; }
        public ICommand TapFavCommand { get; set; }

        public bool _IsFooterVisible = false;
        public bool IsFooterVisible
        {
            get
            {
                return _IsFooterVisible;
            }
            set
            {
                _IsFooterVisible = value;
                OnPropertyChanged("IsFooterVisible");

            }
        }

        public bool _IsFavorite = false;
        public bool IsFavorite
        {
            get
            {
                return _IsFavorite;
            }
            set
            {
                _IsFavorite = value;
                OnPropertyChanged("IsFavorite");
                OnPropertyChanged("FavStatusColor");
            }
        }
        public Color FavStatusColor
        {
            get
            {
                if (IsFavorite)
                {
                    return Color.FromArgb("#00C569");
                }
                return Color.FromArgb("#000000");
            }
        }

        public ProductDetail _ProductDetail = new ProductDetail();
        public ProductDetail ProductDetail
        {
            get
            {
                return _ProductDetail;
            }
            set
            {
                _ProductDetail = value;
                OnPropertyChanged("ProductDetail");
            }
        }

        public ProductDetailViewModel()
        {
            LoadData();
            TapBackCommand = new Command<object>(GoBack);
            TapFavCommand = new Command<Color>(FavItem);
        }

        private async void GoBack(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void FavItem(Color obj)
        {
            IsFavorite = true ? !IsFavorite : IsFavorite;
        }
        public void ChageFooterVisibility(double currentY)
        {
            currentScrollIndex = currentY;
            if (currentScrollIndex > lastScrollIndex)
            {
                IsFooterVisible = false;
            }
            else
            {
                IsFooterVisible = true;
            }
            lastScrollIndex = currentScrollIndex;
        }

        void LoadData()
        {
            ProductDetail = EcommerceServices.Instance.GetProductDetail;
        }
    }
}
