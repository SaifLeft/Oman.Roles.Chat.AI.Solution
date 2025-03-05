namespace MauiKit.Models.Ecommerce
{
    public class ProductListModel
    {
        public List<string> ImageUrls { get; set; }
        public string FeaturedImage => ImageUrls.FirstOrDefault();
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string Price { get; set; }
        public string Details { get; set; }
        public double Qty { get; set; }
    }
}
