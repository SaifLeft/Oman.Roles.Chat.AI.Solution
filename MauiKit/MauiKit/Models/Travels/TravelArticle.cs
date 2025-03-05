
namespace MauiKit.Models
{
    public class TravelArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Excerp { get; set; }
        public string FeaturedImage { get; set; }
        public TravelUser Author { get; set; }
        public PlaceCategory Place { get; set; }
        public string Body { get; set; }
        public string PostedTime { get; set; }
        public List<GalleryItem> Gallery { get; set; }
        public List<TravelArticleComment> Comments { get; set; }
        public double CommentCount { get; set; }
        public double LikeCount { get; set; }
    }

    public class TravelArticleComment
    {
        public TravelUser User { get; set; }
        public string Content { get; set; }
        public float LikeCount { get; set; }
        public string Time { get; set; }
    }

    public class GalleryItem
    {
        public int Id { get; set; }
        public string Image { get; set; }
    }
}
