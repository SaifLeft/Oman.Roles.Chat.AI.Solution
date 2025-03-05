namespace MauiKit.Models.Ecommerce
{
    public class CartItemModel : ObservableObject
    {
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string ProductImage { get; set; }
        public string Details { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                SetProperty(ref _quantity, value);
                SetProperty(ref _amount, value * Price, "Amount");
            }
        }

        public double _price;
        public double Price
        {
            get => _price;
            set
            {
                SetProperty(ref _price, value);
                SetProperty(ref _amount, value * Quantity, "Amount");
            }
        }


        private double _amount;
        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }
    }
}
