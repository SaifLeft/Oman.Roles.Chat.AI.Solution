
namespace MauiKit.Models.Ecommerce
{
    public class ProductDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public List<string> ImageUrls { get; set; }
        public string FeaturedImage => ImageUrls.FirstOrDefault();
        public string Price { get; set; }
        public List<string> Sizes { get; set; }
        public List<ReviewModel> Reviews { get; set; }
        public List<Color> ColorLists { get; set; }
        public string Details { get; set; }
        

    }

    public class ReviewModel
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Review { get; set; }
        public double Rating { get; set; }
        public string Date { get; set; }
    }
}
