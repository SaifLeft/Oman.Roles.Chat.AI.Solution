using System;
namespace MauiKit.Models.News
{
	public class Author
	{
		public int Id { get; set; }
		public string Name { get; set; }
        public string Avatar { get; set; }
        public double ArticleCount { get; set; }
		public double Rating { get; set; }
		public bool IsFollowed { get; set; }
		public bool IsFeatured{ get; set; }
	}
}

