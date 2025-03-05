using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using MauiKit.Models.News;

using MauiKit.Views;

namespace MauiKit.Services
{
    public class MockNewsServices
    {
        static MockNewsServices _instance;

        public static MockNewsServices Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MockNewsServices();

                return _instance;
            }
        }

        public List<TrendingCategory> GetTrendingCategories
        {
            get
            {
                return new List<TrendingCategory>
                {
                    new TrendingCategory { CategoryId = 1, CategoryName = "Sports", Thumbnail = AppSettings.ImageServerPath +  "news/cat-sports.jpg" },
                    new TrendingCategory { CategoryId = 2, CategoryName = "Politices", Thumbnail = AppSettings.ImageServerPath +  "news/cat-politices.jpg" },
                    new TrendingCategory { CategoryId = 3, CategoryName = "Natures", Thumbnail = AppSettings.ImageServerPath +  "news/cat-nature.jpg" },
                    new TrendingCategory { CategoryId = 4, CategoryName = "Health", Thumbnail = AppSettings.ImageServerPath +  "news/cat-health.jpg" },
                    new TrendingCategory { CategoryId= 5, CategoryName = "Travels", Thumbnail = AppSettings.ImageServerPath +  "news/cat-travel.jpg" },
                    new TrendingCategory { CategoryId = 6, CategoryName = "Business", Thumbnail = AppSettings.ImageServerPath +  "news/cat-business.jpg" },
                    new TrendingCategory { CategoryId= 7, CategoryName = "Marketing", Thumbnail = AppSettings.ImageServerPath +  "news/cat-marketing.jpg" }
                };
            }
        }
        public List<string> GetTags
        {
            get
            {
                return new List<string>
                {
                    "#today",
                    "#health",
                    "#politics",
                    "#nature",
                    "#music",
                    "#arts",
                    "#marketing",
                    "#business",
                    "#science",
                    "#world",
                    "#sports",
                    "#party"
                };
            }
        }

        public List<NewsCategory> GetCategories
        {
            get
            {
                return new List<NewsCategory>
                {
                    new NewsCategory {Name = "All", ImageUrl= AppSettings.ImageServerPath +  "news/cat-all.jpg" },
                    new NewsCategory {Name = "Sports", ImageUrl= AppSettings.ImageServerPath +  "news/cat-sports.jpg" },
                    new NewsCategory {Name = "Health", ImageUrl= AppSettings.ImageServerPath +  "news/cat-health.jpg" },
                    new NewsCategory {Name = "Politics", ImageUrl = AppSettings.ImageServerPath +  "news/cat-politices.jpg" },
                    new NewsCategory {Name = "Business", ImageUrl = AppSettings.ImageServerPath +  "news/cat-business.jpg" },
                    new NewsCategory {Name = "Music", ImageUrl = AppSettings.ImageServerPath +  "news/cat-music.jpg" },
                    new NewsCategory {Name = "Marketing", ImageUrl = AppSettings.ImageServerPath +  "news/cat-marketing.jpg"},
                    new NewsCategory {Name = "Natures", ImageUrl = AppSettings.ImageServerPath +  "news/cat-nature.jpg" },
                    new NewsCategory {Name = "Arts", ImageUrl = AppSettings.ImageServerPath +  "news/cat-arts.jpg" },
                    new NewsCategory {Name = "Travels", ImageUrl = AppSettings.ImageServerPath +  "news/cat-travel.jpg" },
                    new NewsCategory {Name = "Foods", ImageUrl = AppSettings.ImageServerPath +  "news/cat-food.jpg" },
                    new NewsCategory {Name = "Styles", ImageUrl = AppSettings.ImageServerPath +  "news/cat-style.jpg" }
                };
            }
        }

        public List<Article> GetLatestArticles
        {
            get
            {
                return new List<Article>
                {
                    new Article {
                    Id = "001",
                    Title = "Massa ultricies mi quis hendrerit dolor magna eget.",
                    Subtitle = "Ut venenatis tellus in metus vulputate eu. Enim neque volutpat ac tincidunt vitae semper quis.",
                    ImageURL = AppSettings.ImageServerPath +  "news/01.jpg",
                    Category = "Business",
                    Time = "8m ago",
                    LikeCount = 25,
                    CommentCount = 6
                },
                    new Article {
                        Id = "002",
                        Title = "Auctor elit sed vulputate mi sit amet mauris.",
                        Subtitle = "Habitasse platea dictumst vestibulum rhoncus est. Et ligula ullamcorper malesuada proin libero nunc.",
                        ImageURL = AppSettings.ImageServerPath +  "news/02.jpg",
                        Category = "Sports",
                        Time = "12m ago",
                        LikeCount = 40,
                        CommentCount = 10
                    },
                    new Article {
                        Id = "003",
                        Title = "Nibh venenatis cras sed felis eget velit aliquet.",
                        Subtitle = "Sollicitudin ac orci phasellus egestas. Nulla facilisi cras fermentum odio eu feugiat pretium nibh.",
                        ImageURL = AppSettings.ImageServerPath +  "news/03.jpg",
                        Category = "Science",
                        Time = "16m ago",
                        LikeCount = 99,
                        CommentCount = 143
                    },
                    new Article {
                        Id = "004",
                        Title = "Massa tempor nec feugiat nisl pretium fusce.",
                        Subtitle = "Tellus in metus vulputate eu scelerisque felis imperdiet. Orci eu lobortis elementum nibh tellus molestie nunc non blandit.",
                        ImageURL = AppSettings.ImageServerPath +  "news/04.jpg",
                        Category = "Politics",
                        Time = "23m ago",
                        LikeCount = 33,
                        CommentCount = 12
                    },
                    new Article {
                        Id = "005",
                        Title = "Nullam vehicula ipsum a arcu cursus vitae congue mauris rhoncus.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/05.jpg",
                        Category = "Business",
                        Time = "25m ago",
                        LikeCount = 100,
                        CommentCount = 1345
                    },
                    new Article {
                        Id = "006",
                        Title = "Cursus metus aliquam eleifend mi in nulla posuere.",
                        Subtitle = "Volutpat lacus laoreet non curabitur gravida arcu. Quis imperdiet massa tincidunt nunc pulvinar sapien et.",
                        ImageURL = AppSettings.ImageServerPath +  "news/06.jpg",
                        Category = "Sports",
                        Time = "26m ago",
                        LikeCount = 121,
                        CommentCount = 133
                    },
                    new Article {
                        Id = "007",
                        Title = "Consequat interdum varius sit amet mattis vulputate enim nulla.",
                        Subtitle = "Elementum integer enim neque volutpat. Sit amet consectetur adipiscing elit pellentesque habitant morbi tristique senectus.",
                        ImageURL = AppSettings.ImageServerPath +  "news/07.jpg",
                        Category = "Science",
                        Time = "12 Nov, 2022",
                        LikeCount = 534,
                        CommentCount = 234
                    },
                    new Article {
                        Id = "008",
                        Title = "Augue ut lectus arcu bibendum at varius vel pharetra vel.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/08.jpg",
                        Category = "Politics",
                        Time = "10 Nov, 2022",
                        LikeCount = 275,
                        CommentCount = 332
                    }
                };
            }
        }

        public List<Article> GetRecentArticles
        {
            get 
            {
                return new List<Article>
                {
                    new Article {
                    Id = "009",
                    Title = "Dictum fusce ut placerat orci nulla pellentesque.",
                    Subtitle = "Volutpat lacus laoreet non curabitur gravida arcu. Quis imperdiet massa tincidunt nunc pulvinar sapien et.",
                    ImageURL ="https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/news/09.jpg",
                    Category = "Business",
                    Time = "10m ago",
                    LikeCount = 275,
                    CommentCount = 332
                },
                    new Article {
                        Id = "010",
                        Title = "Facilisi cras fermentum odio eu feugiat pretium nibh ipsum consequat.",
                        Subtitle = "Elementum integer enim neque volutpat. Sit amet consectetur adipiscing elit pellentesque habitant morbi tristique senectus.",
                        ImageURL = AppSettings.ImageServerPath +  "news/10.jpg",
                        Category = "Sports",
                        Time = "12m ago",
                        LikeCount = 534,
                        CommentCount = 234
                    },
                    new Article {
                        Id = "011",
                        Title = "Volutpat odio facilisis mauris sit amet massa vitae.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/11.jpg",
                        Category = "Science",
                        Time = "16m ago",
                        LikeCount = 121,
                        CommentCount = 133
                    },
                    new Article {
                        Id = "012",
                        Title = "Blandit volutpat maecenas volutpat blandit aliquam etiam.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/12.jpg",
                        Category = "Politics",
                        Time = "23m ago",
                        LikeCount = 99,
                        CommentCount = 143
                    }
                };
            }
        }

        public List<Channel> GetTopChannels
        {
            get
            {
                return new List<Channel>
                {
                    new Channel 
                    {
                        Name = "BBC News",
                        Tagline = "Tempus quam pellentesque nec nam aliquam sem",
                        ImageIcon = AppSettings.ImageServerPath +  "news/bbc-news.jpg",
                        Language = "English"
                    },
                    new Channel 
                    {
                        Name = "Fox News",
                        Tagline = "Tempus quam pellentesque nec nam aliquam sem",
                        ImageIcon = AppSettings.ImageServerPath +  "news/fox-news.png",
                        Language = "Bangla"
                    },
                    new Channel 
                    {
                        Name = "CNBC News",
                        Tagline = "Tempus quam pellentesque nec nam aliquam sem",
                        ImageIcon = AppSettings.ImageServerPath +  "news/cnbc-news.png",
                        Language = "Hindi"
                    }
                };
            }
        }

        public Article GetArticleDetail
        {
            get
            {
                return new Article
                {
                    Id = "001",
                    Title = "UEFA Champions League and Europa League changes",
                    Subtitle = "Volutpat lacus laoreet non curabitur gravida arcu. Quis imperdiet massa tincidunt nunc pulvinar sapien et.",
                    ImageURL = AppSettings.ImageServerPath +  "news/01.jpg",
                    ChannelName = "BBC News",
                    ChannelImage = AppSettings.ImageServerPath +  "news/bbc-news.jpg",
                    Author = "TLS Software",
                    Category = "Business",
                    Time = "10m ago",
                    Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Ipsum dolor sit amet consectetur adipiscing elit. Nunc mi ipsum faucibus vitae aliquet. Ipsum dolor sit amet consectetur adipiscing elit duis tristique sollicitudin. Odio ut sem nulla pharetra diam sit. Placerat in egestas erat imperdiet sed euismod nisi. Donec ultrices tincidunt arcu non sodales neque sodales ut. Ultrices dui sapien eget mi proin sed libero. Lacus sed turpis tincidunt id aliquet risus. Sollicitudin nibh sit amet commodo nulla. Amet porttitor eget dolor morbi non arcu risus quis varius. Vulputate odio ut enim blandit. In fermentum posuere urna nec tincidunt praesent semper feugiat. Varius sit amet mattis vulputate enim nulla aliquet. Quisque sagittis purus sit amet volutpat consequat mauris. Montes nascetur ridiculus mus mauris vitae. Amet mauris commodo quis imperdiet massa tincidunt. Sed risus ultricies tristique nulla aliquet enim tortor at." +
                                "\n\n" + "Condimentum mattis pellentesque id nibh tortor id aliquet. Suspendisse faucibus interdum posuere lorem ipsum. Platea dictumst quisque sagittis purus sit amet volutpat. Proin libero nunc consequat interdum varius sit amet mattis. Vivamus arcu felis bibendum ut tristique et. Pulvinar elementum integer enim neque volutpat ac tincidunt. Ornare lectus sit amet est placerat. Sed arcu non odio euismod lacinia at. Placerat orci nulla pellentesque dignissim enim sit. Eros donec ac odio tempor orci dapibus ultrices. Dolor sit amet consectetur adipiscing elit ut aliquam purus sit. Dolor sit amet consectetur adipiscing elit duis tristique sollicitudin nibh. Leo a diam sollicitudin tempor id eu nisl nunc. Mus mauris vitae ultricies leo integer malesuada nunc vel." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                    Comments = new List<Comment>()
                    {
                        new Comment()
                        {
                            User = "@fahimstudio",
                            UserAvatar = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                            LikeCount = 12,
                            Time = "10 minutes ago"
                        },
                        new Comment()
                        {
                            User = "@Tasalanea",
                            UserAvatar = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                            LikeCount = 15,
                            Time = "12 minutes ago"
                        },
                    }
                };
            }
        }

        public List<Article> GetBookmarkedArticles
        {
            get
            {
                return new List<Article>
                {
                    new Article 
                    {
                        Id = "009",
                        Title = "Dictum fusce ut placerat orci nulla pellentesque.",
                        Subtitle = "Volutpat lacus laoreet non curabitur gravida arcu. Quis imperdiet massa tincidunt nunc pulvinar sapien et.",
                        ImageURL ="https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/news/05.jpg",
                        Category = "Business",
                        Time = "10m ago",
                        LikeCount = 275,
                        CommentCount = 332
                    },
                    new Article {
                        Id = "010",
                        Title = "Facilisi cras fermentum odio eu feugiat pretium nibh ipsum consequat.",
                        Subtitle = "Elementum integer enim neque volutpat. Sit amet consectetur adipiscing elit pellentesque habitant morbi tristique senectus.",
                        ImageURL = AppSettings.ImageServerPath +  "news/06.jpg",
                        Category = "Sports",
                        Time = "12m ago",
                        LikeCount = 534,
                        CommentCount = 234
                    },
                    new Article {
                        Id = "011",
                        Title = "Volutpat odio facilisis mauris sit amet massa vitae.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/07.jpg",
                        Category = "Science",
                        Time = "16m ago",
                        LikeCount = 121,
                        CommentCount = 133
                    },
                    new Article {
                        Id = "012",
                        Title = "Blandit volutpat maecenas volutpat blandit aliquam etiam.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/08.jpg",
                        Category = "Politics",
                        Time = "23m ago",
                        LikeCount = 99,
                        CommentCount = 143
                    },
                    new Article {
                        Id = "013",
                        Title = "Facilisi cras fermentum odio eu feugiat pretium nibh ipsum consequat.",
                        Subtitle = "Elementum integer enim neque volutpat. Sit amet consectetur adipiscing elit pellentesque habitant morbi tristique senectus.",
                        ImageURL = AppSettings.ImageServerPath +  "news/09.jpg",
                        Category = "Sports",
                        Time = "12m ago",
                        LikeCount = 534,
                        CommentCount = 234
                    },
                    new Article {
                        Id = "014",
                        Title = "Volutpat odio facilisis mauris sit amet massa vitae.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/10.jpg",
                        Category = "Science",
                        Time = "16m ago",
                        LikeCount = 121,
                        CommentCount = 133
                    },
                    new Article {
                        Id = "015",
                        Title = "Blandit volutpat maecenas volutpat blandit aliquam etiam.",
                        Subtitle = "Tempus quam pellentesque nec nam aliquam sem. Sed faucibus turpis in eu mi bibendum neque.",
                        ImageURL = AppSettings.ImageServerPath +  "news/11.jpg",
                        Category = "Politics",
                        Time = "23m ago",
                        LikeCount = 99,
                        CommentCount = 143
                    }
                };
            }
        }

        public List<Author> GetAllAuthors
        {
            get
            {
                return new List<Author>
                {
                    new Author
                    {
                        Id = 001,
                        Name = "Junayed",
                        Avatar = AppSettings.ImageServerPath +  "news/users/a1.jpg",
                        ArticleCount = 152,
                        Rating = 4.8,
                        IsFollowed = false,
                        IsFeatured = false
                    },
                    new Author
                    {
                        Id = 002,
                        Name = "Sakila",
                        Avatar = AppSettings.ImageServerPath +  "news/users/a2.jpg",
                        ArticleCount = 565,
                        Rating = 4.5,
                        IsFollowed = false,
                        IsFeatured = false
                    },
                    new Author
                    {
                        Id = 003,
                        Name = "Sohanur",
                        Avatar = AppSettings.ImageServerPath +  "news/users/a3.jpg",
                        ArticleCount = 256,
                        Rating = 4.7,
                        IsFollowed = false,
                        IsFeatured = false
                    },
                    new Author
                    {
                        Id = 004,
                        Name = "Roberts",
                        Avatar = AppSettings.ImageServerPath +  "news/users/a4.jpg",
                        ArticleCount = 132,
                        Rating = 4.8,
                        IsFollowed = false,
                        IsFeatured = true
                    },
                    new Author
                    {
                        Id = 005,
                        Name = "Nura Lineon",
                        Avatar = AppSettings.ImageServerPath +  "news/users/a5.jpg",
                        ArticleCount = 252,
                        Rating = 5.0,
                        IsFollowed = false,
                        IsFeatured = true
                    },
                    new Author
                    {
                        Id = 006,
                        Name = "Anjelo Reo",
                        Avatar = AppSettings.ImageServerPath +  "news/users/a6.jpg",
                        ArticleCount = 212,
                        Rating = 4.7,
                        IsFollowed = false,
                        IsFeatured = true
                    },
                    new Author
                    {
                        Id = 007,
                        Name = "Robinu Thapa",
                        Avatar = AppSettings.ImageServerPath +  "news/users/a7.jpg",
                        ArticleCount = 253,
                        Rating = 5.0,
                        IsFollowed = true,
                        IsFeatured = true
                    }
                };
            }
        }
    }
}
