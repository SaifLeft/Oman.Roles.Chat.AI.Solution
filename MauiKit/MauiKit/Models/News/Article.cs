using System;
namespace MauiKit.Models.News
{
    public class Article
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImageURL { get; set; }
        public string ChannelName { get; set; }
        public string ChannelImage { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Body { get; set; }
        public string Time { get; set; }
        public List<Comment> Comments { get; set; }
        public double CommentCount { get; set; }
        public double LikeCount { get; set; }
    }

    public class Comment
    {
        public string User { get; set; }
        public string UserAvatar { get; set; }
        public string Content { get; set; }
        public float LikeCount { get; set; }
        public string Time { get; set; }
    }
}
