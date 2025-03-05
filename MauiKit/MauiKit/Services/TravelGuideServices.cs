
using System.Collections.ObjectModel;

namespace MauiKit.Services
{
    public class TravelGuideServices
    {

        static TravelGuideServices _instance;
        public static TravelGuideServices Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TravelGuideServices();

                return _instance;
            }
        }


        internal PlaceCategory place1 = new()
        {
            Id = 1,
            CategoryName = "Mountain",
            Icon = AppSettings.ImageServerPath +  "travel/location-1.png"
        };
        internal PlaceCategory place2 = new()
        {
            Id = 2,
            CategoryName = "Lakes",
            Icon = AppSettings.ImageServerPath +  "travel/location-2.png"
        };
        internal PlaceCategory place3 = new()
        {
            Id = 3,
            CategoryName = "Beach",
            Icon = AppSettings.ImageServerPath +  "travel/location-3.png"
        };
        internal PlaceCategory place4 = new()
        {
            Id = 4,
            CategoryName = "Sea",
            Icon = AppSettings.ImageServerPath +  "travel/location-4.png"
        };
        internal PlaceCategory place5 = new()
        {
            Id = 5,
            CategoryName = "Hills",
            Icon = AppSettings.ImageServerPath +  "travel/location-5.png"
        };
        internal PlaceCategory place6 = new()
        {
            Id = 6,
            CategoryName = "Lankmarks",
            Icon = AppSettings.ImageServerPath +  "travel/location-6.png"
        };
        internal PlaceCategory place7 = new()
        {
            Id = 7,
            CategoryName = "Forest",
            Icon = AppSettings.ImageServerPath +  "travel/location-7.png"
        };
        public List<PlaceCategory> PlaceCategories
        {
            get
            {
                return new List<PlaceCategory>
                {
                    place1, place2, place3, place4, place5, place6, place7
                };
            }
        }

        readonly TravelUser user1 = new TravelUser
        {
            Id = 001,
            FullName = "Alan Cordova",
            NickName = "@alancordova",
            Status = "Online",
            Avatar = AppSettings.ImageServerPath +  "avatars/150-16.jpg",
            Color = Color.FromArgb("#FFE0EC")
        };
        readonly TravelUser user2 = new()
        {
            Id = 002,
            FullName = "Cecily Trujillo",
            NickName = "@cecily",
            Status = "Online",
            Avatar = AppSettings.ImageServerPath +  "avatars/150-17.jpg",
            Color = Color.FromArgb("#BFE9F2")
        };
        readonly TravelUser user3 = new()
        {
            Id = 003,
            FullName = "Eathan Sheridan",
            Status = "Busy",
            NickName = "@eathan",
            Avatar = AppSettings.ImageServerPath +  "avatars/150-18.jpg",
            Color = Color.FromArgb("#FFD6C4")
        };
        readonly TravelUser user4 = new()
        {
            Id = 004,
            FullName = "Komal Orr",
            Status = "Busy",
            NickName = "@kormal",
            Avatar = AppSettings.ImageServerPath +  "avatars/150-19.jpg",
            Color = Color.FromArgb("#C3C1E6")
        };
        readonly TravelUser user5 = new()
        {
            Id = 005,
            FullName = "Sariba Abood",
            Status = "Offline",
            NickName = "@sariba",
            Avatar = AppSettings.ImageServerPath +  "avatars/150-20.jpg",
            Color = Color.FromArgb("#FFE0EC")
        };
        public List<TravelUser> GetUsers()
        {
            return new List<TravelUser>
            {
                user1, user2, user3, user4, user5
            };
        }

        internal List<GalleryItem> Gallery
        {
            get
            {
                return new List<GalleryItem>
                {
                    new GalleryItem { Id = 1, Image = AppSettings.ImageServerPath +  "travel/gallery-1.jpg" },
                    new GalleryItem { Id = 2, Image = AppSettings.ImageServerPath +  "travel/gallery-2.jpg" },
                    new GalleryItem { Id = 3, Image = AppSettings.ImageServerPath +  "travel/gallery-3.jpg" },
                    new GalleryItem { Id = 4, Image = AppSettings.ImageServerPath +  "travel/gallery-4.jpg" },
                    new GalleryItem { Id = 5, Image = AppSettings.ImageServerPath +  "travel/gallery-5.jpg" },
                    new GalleryItem { Id = 6, Image = AppSettings.ImageServerPath +  "travel/gallery-6.jpg" },
                    new GalleryItem { Id = 7, Image = AppSettings.ImageServerPath +  "travel/gallery-7.jpg" },
                    new GalleryItem { Id = 8, Image = AppSettings.ImageServerPath +  "travel/gallery-8.jpg" },
                    new GalleryItem { Id = 9, Image = AppSettings.ImageServerPath +  "travel/gallery-9.jpg" }
                };
            }
        }

        internal List<TravelArticleComment> TravelArticleComments
        {
            get
            {
                return new List<TravelArticleComment>
                {
                    new TravelArticleComment()
                    {
                        User = this.user4,
                        Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                        LikeCount = 12,
                        Time = "10 minutes ago"
                    },
                    new TravelArticleComment()
                    {
                        User = this.user5,
                        Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                        LikeCount = 15,
                        Time = "12 minutes ago"
                    },
                };
            }
        }

        public List<TravelArticle> PopularTravelGuides
        {
            get
            {
                return new List<TravelArticle>
                {
                    new TravelArticle
                    {
                        Id = 1,
                        Title = "Above the Clouds:" + "\n" + "Tenerif Travel Guide",
                        Excerp = "Sith sky landspace, Simple Building, and perfect for photos.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-1.jpg",
                        Author = this.user1,
                        Place = this.place1,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "5 minutes ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 489,
                        LikeCount = 747
                    },
                    new TravelArticle
                    {
                        Id = 2,
                        Title = "The far east is a land of adventures",
                        Excerp = "Sed ut in perspiciatis unde omnis iste natus.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-2.jpg",
                        Author = this.user2,
                        Place = this.place2,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "1 hour ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 964,
                        LikeCount = 925
                    },
                    new TravelArticle
                    {
                        Id = 3,
                        Title = "The trip that didn't take place twice",
                        Excerp = "Sith sky landspace, Simple Building, and perfect for photos.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-3.jpg",
                        Author = this.user1,
                        Place = this.place3,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "2 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 478,
                        LikeCount = 5643
                    },
                    new TravelArticle
                    {
                        Id = 4,
                        Title =  "Morning Coffee Smells Sweet",
                        Excerp = "Sed ut in perspiciatis unde omnis iste natus.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-4.jpg",
                        Author = this.user3,
                        Place = this.place4,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "2 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 567,
                        LikeCount = 2333
                    },
                    new TravelArticle
                    {
                        Id = 5,
                        Title =  "Christmas Came Early This Year",
                        Excerp = "Sith sky landspace, Simple Building, and perfect for photos.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-5.jpg",
                        Author = this.user1,
                        Place = this.place5,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "3 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 565,
                        LikeCount = 433
                    },
                    new TravelArticle
                    {
                        Id = 6,
                        Title =  "Basic Inner Workings of Camera",
                        Excerp = "Sed ut in perspiciatis unde omnis iste natus.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-6.jpg",
                        Author = this.user2,
                        Place = this.place6,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "3 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 100,
                        LikeCount = 764
                    },
                    new TravelArticle
                    {
                        Id = 7,
                        Title =  "Pleasant Colors in Garden",
                        Excerp = "Sith sky landspace, Simple Building, and perfect for photos.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-7.jpg",
                        Author = this.user1,
                        Place = this.place7,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "5 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 543,
                        LikeCount = 5463
                    },
                    new TravelArticle
                    {
                        Id = 8,
                        Title =  "Blooming Flowers in The House",
                        Excerp = "Sed ut in perspiciatis unde omnis iste natus.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-8.jpg",
                        Author = this.user3,
                        Place = this.place5,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "7 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 865,
                        LikeCount = 345
                    },
                    new TravelArticle
                    {
                        Id = 9,
                        Title =  "Older Cars Never Out of Style",
                        Excerp = "Sith sky landspace, Simple Building, and perfect for photos.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-9.jpg",
                        Author = this.user3,
                        Place = this.place1,
                        Gallery = this.Gallery,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        PostedTime = "8 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 443,
                        LikeCount = 755
                    }
                };
            }
        }

        public List<TravelArticle> MyArticles
        {
            get
            {
                return new List<TravelArticle>
                {
                    new TravelArticle
                    {
                        Id = 1,
                        Title = "Above the Clouds:" + "\n" + "Tenerif Travel Guide",
                        Excerp = "Sed ut in perspiciatis unde omnis iste natus.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-5.jpg",
                        Author = this.user1,
                        Place = this.place4,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "5 minutes ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 100,
                        LikeCount = 1344
                    },
                    new TravelArticle
                    {
                        Id = 2,
                        Title = "The far east is a land of adventures",
                        Excerp = "Sith sky landspace, Simple Building, and perfect for photos.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-6.jpg",
                        Author = this.user1,
                        Place = this.place1,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "30 minutes ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 675,
                        LikeCount = 448
                    },
                    new TravelArticle
                    {
                        Id = 3,
                        Title = "The trip that didn't take place twice",
                        Excerp = "Sed ut in perspiciatis unde omnis iste natus.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-7.jpg",
                        Author = this.user1,
                        Place = this.place2,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        Gallery = this.Gallery,
                        PostedTime = "2 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 232,
                        LikeCount = 977
                    },
                    new TravelArticle
                    {
                        Id = 4,
                        Title =  "Minimalist Interior Makeover",
                        Excerp = "Sith sky landspace, Simple Building, and perfect for photos.",
                        FeaturedImage = AppSettings.ImageServerPath +  "travel/article-image-8.jpg",
                        Author = this.user1,
                        Place = this.place3,
                        Gallery = this.Gallery,
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor dignissim convallis aenean et tortor at risus viverra adipiscing. Maecenas pharetra convallis posuere morbi leo urna molestie. Tellus molestie nunc non blandit massa. Tempor orci eu lobortis elementum nibh. Posuere sollicitudin aliquam ultrices sagittis orci. Mi ipsum faucibus vitae aliquet nec ullamcorper sit amet risus. Lorem dolor sed viverra ipsum nunc aliquet bibendum. Ut porttitor leo a diam sollicitudin tempor. Eu facilisis sed odio morbi. Semper feugiat nibh sed pulvinar proin gravida. Ullamcorper sit amet risus nullam eget felis. Turpis massa sed elementum tempus egestas sed sed risus. Non nisi est sit amet facilisis magna etiam tempor orci. Nibh praesent tristique magna sit amet purus gravida. Lacinia quis vel eros donec ac odio tempor orci. Lectus sit amet est placerat in. Euismod quis viverra nibh cras pulvinar mattis nunc. Auctor elit sed vulputate mi. Turpis in eu mi bibendum neque egestas." +
                                "\n\n" + "Tortor dignissim convallis aenean et tortor at risus. Sed nisi lacus sed viverra tellus in hac. Elementum nisi quis eleifend quam adipiscing vitae. In hac habitasse platea dictumst quisque sagittis purus sit. Hendrerit gravida rutrum quisque non tellus orci. Viverra suspendisse potenti nullam ac tortor vitae. Ut aliquam purus sit amet luctus venenatis lectus magna. Consequat nisl vel pretium lectus quam id leo in vitae. Feugiat in ante metus dictum at tempor commodo ullamcorper a. Mauris ultrices eros in cursus turpis. Ultrices sagittis orci a scelerisque purus semper eget. Eu turpis egestas pretium aenean pharetra magna ac. Sapien et ligula ullamcorper malesuada. Pellentesque dignissim enim sit amet. Ac tortor dignissim convallis aenean et tortor at." +
                                "\n\n" + "Felis bibendum ut tristique et egestas quis ipsum suspendisse ultrices. Viverra accumsan in nisl nisi. In massa tempor nec feugiat nisl pretium. Et netus et malesuada fames ac turpis egestas maecenas. Ut enim blandit volutpat maecenas volutpat blandit aliquam etiam erat. Est sit amet facilisis magna etiam tempor orci eu. Et netus et malesuada fames ac turpis egestas sed tempus. Maecenas pharetra convallis posuere morbi leo urna. Enim nec dui nunc mattis. Convallis a cras semper auctor neque.",
                        PostedTime = "3 hours ago",
                        Comments = this.TravelArticleComments,
                        CommentCount = 897,
                        LikeCount = 5677
                    }
                };
            }
        }

        public List<TravelMessage> GetRecentMessages
        {
            get
            {
                return new List<TravelMessage>
                {
                    new TravelMessage
                    {
                      Sender = user1,
                      Time = "18:32",
                      Text = "Hey there! What\'s up? Is everything ok?",
                    },
                    new TravelMessage
                      {
                        Sender = user5,
                        Time = "14:05",
                        Text = "Can I call you back later?, I\'m in a meeting.",
                      },
                      new TravelMessage
                      {
                        Sender = user3,
                        Time = "14:00",
                        Text = "Yeah. Do you have any good song to recommend?",
                      },
                      new TravelMessage
                      {
                        Sender = user2,
                        Time = "13:35",
                        Text = "Hi! I went shopping today and found a nice t-shirt.",
                      },
                      new TravelMessage
                      {
                        Sender = user4,
                        Time= "12:11",
                        Text= "I passed you on the ride to work today, see you later.",
                      }
                };
            }
        }
        public List<TravelMessage> GetConversationDetail(TravelUser sender)
        {
            return new List<TravelMessage> {
              new TravelMessage
              {
                Sender = null,
                Receiver = user2,
                Time = "18:01",
                Text = "Hi, Alan",
              },
              new TravelMessage
              {
                Sender = null,
                Receiver = user2,
                Time = "18:02",
                Text = "I've tried the app",
              },
              new TravelMessage
              {
                Sender = sender,
                Receiver = user2,
                Time = "18:05",
                Text = "Really?",
              },
              new TravelMessage
              {
                Sender = null,
                Receiver = user2,
                Time = "18:07",
                Text = "Yeah, It's really good!",
              },
              new TravelMessage
              {
                Sender= sender,
                Receiver = user2,
                Time = "18:8",
                Text= "Great to know about that",
              }
            };
        }

    }
}