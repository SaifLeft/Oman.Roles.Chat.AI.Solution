namespace MauiKit.ViewModels.Dashboards;
public partial class DashboardArticleViewModel : ObservableObject
{
    public DashboardArticleViewModel()
    {
        LoadData();
    }

    void LoadData()
    {
        Task.Run(async () =>
        {
            IsBusy = true;
            // await api call;
            await Task.Delay(500);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                ArticleLists1 = new ObservableCollection<ArticleData>
                {
                    new ArticleData
                    {
                        Id = 001,
                        Title =  "Basic Inner Workings of Camera",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "ACTUALITY",
                        SectionColor = Color.FromRgba("#ff2602"),
                        Author =  "TLS SOFTWARE",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a1.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_01.jpg",
                        VideoUrl = "https://archive.org/download/Sintel/sintel-2048-stereo_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "John Doe",
                        When =  "JAN 6, 2017",
                        Followers =  "112",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jhon Deo",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Greatest article I have ever read in my life.",
                                Body =  "Write a one or two sentence summary of the goal or project you want to complete.",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "29 Dec, 2019",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Then write every idea you associate with the goal or project on separate pieces of paper sticky notes are ideal. Don\u0027t self edit at this point, write everything that comes to mind.",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Spread all the pieces of paper onto a table, a desk, a bed, or even the floor.",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "2.1k"
                    },
                    new ArticleData
                    {
                        Id = 002,
                        Title =  "Vintage Beauty in Decorations",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "SPORTS",
                        SectionColor = Color.FromRgba("#ffc000"),
                        Author =  "Julia Grant",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a2.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_02.jpg",
                        VideoUrl = "https://archive.org/download/ElephantsDream/ed_hd_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Julia Grant",
                        When =  "FEB 13, 2017",
                        Followers =  "340",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Spread all the pieces of paper onto a table, a desk, a bed, or even the floor.",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Sort the ideas by category some will be tasks to do, others will be equipment or training you need.",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "12k"
                    },
                    new ArticleData
                    {
                        Id = 003,
                        Title =  "Christmas Came Early This Year",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Maecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "FREE TIME",
                        SectionColor = Color.FromRgba("#707525"),
                        Author =  "Jhon Deo",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a3.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_03.jpg",
                        VideoUrl = "https://archive.org/download/Sintel/sintel-2048-stereo_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Jhon Deo",
                        When =  "FEB 21, 2017",
                        Followers =  "120",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "3.8k"
                    }
                };

                ArticleLists2 = new ObservableCollection<ArticleData>
                {
                    new ArticleData
                    {
                        Id = 004,
                        Title =  "Morning Coffee Smells Sweet",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "SCIENCE",
                        SectionColor = Color.FromRgba("#2e66a1"),
                        Author =  "TLS SOFTWARE",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a4.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_04.jpg",
                        VideoUrl = "https://archive.org/download/ElephantsDream/ed_hd_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Janis Spector",
                        When =  "12/8/2018",
                        Followers =  "345",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "48k"
                    },
                    new ArticleData
                    {
                        Id = 005,
                        Title =  "Pleasant Colors in Garden",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "NATURE",
                        SectionColor = Color.FromRgba("#49ab81"),
                        Author =  "David Son",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a5.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_05.jpg",
                        VideoUrl = "https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Julia Grant",
                        When =  "29 Dec, 2019",
                        Followers =  "2k",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "2.1k"
                    },
                    new ArticleData
                    {
                        Id = 006,
                        Title =  "My Shiny New Backpack",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "HEALTH",
                        SectionColor = Color.FromRgba("#e78b28"),
                        Author =  "Danielle Schneider",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a6.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_06.jpg",
                        VideoUrl = "https://archive.org/download/ElephantsDream/ed_hd_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Danielle Schneider",
                        When =  "JUN 12, 2017",
                        Followers =  "235",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "4.7K"
                    }
                };

                ArticleLists3 = new ObservableCollection<ArticleData>
                {
                    new ArticleData
                    {
                        Id = 007,
                        Title =  "Blooming Flowers in The House",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "FREE TIME",
                        SectionColor = Color.FromRgba("#ff647e"),
                        Author =  "Jhon Deo",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a7.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_07.jpg",
                        VideoUrl = "https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Jhon Deo",
                        When =  "FEB 21, 2017",
                        Followers =  "120",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "3.8k"
                    },
                    new ArticleData
                    {
                        Id = 008,
                        Title =  "Older Cars Never Out of Style",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "SCIENCE",
                        SectionColor = Color.FromRgba("#3680ab"),
                        Author =  "TLS SOFTWARE",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a8.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_08.jpg",
                        VideoUrl = "https://archive.org/download/ElephantsDream/ed_hd_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Janis Spector",
                        When =  "12/8/2018",
                        Followers =  "345",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "48k"
                    },
                    new ArticleData
                    {
                        Id = 009,
                        Title =  "Minimalist Interior Makeover",
                        Subtitle =  "Sed ut in perspiciatis unde omnis iste natus.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent et aliquet nunc.\nSed ultricies sed augue sit amet maximus. In vel tellus sed ipsum volutpat venenatis et sit amet diam. Suspendisse feugiat mollis nibh, in facilisis diam convallis sit amet.\n\nMaecenas lectus turpis, rhoncus et est at, lacinia placerat urna. Praesent malesuada consectetur justo, scelerisque fermentum enim lobortis ullamcorper. Duis commodo sit amet ligula vitae luctus. Nulla commodo ipsum a lorem efficitur luctus.",
                        Section = "NATURE",
                        SectionColor = Color.FromRgba("#49ab81"),
                        Author =  "David Son",
                        Avatar = AppSettings.ImageServerPath +  "avatars/a9.jpg",
                        BackgroundImage =  "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/articles/article_09.jpg",
                        VideoUrl = "https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4",
                        Quote =  "Donec euismod nulla et sem lobortis ultrices. Cras id imperdiet metus. Sed congue luctus arcu.",
                        QuoteAuthor =  "Julia Grant",
                        When =  "29 Dec, 2019",
                        Followers =  "2k",
                        Comments = new List<ArticleCommentData>
                        {
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Alice Russell",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user1.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 2,
                                When = "Apr 16",
                                IsStared =  false,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "David Son",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user2.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "3 minutes ago",
                                IsStared =  true,
                                IsRead =  false
                            },
                            new ArticleCommentData
                            {
                                From = new ArticleCommentOwnerData
                                {
                                    Name = "Jessica Park",
                                    Avatar = AppSettings.ImageServerPath +  "avatars/user3.png"
                                },
                                Title =  "Absolutely love them! Can't stop reading!",
                                Body =  "Now you are ready to enter the items in an organized fashion into your project management software",
                                HasAttachment =  true,
                                ThreadCount = 10,
                                When = "July 8",
                                IsStared =  true,
                                IsRead =  true
                            }
                        },
                        Likes = "2.1k"
                    }
                };

                IsBusy = false;
            });
        });
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _articleLists1;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _articleLists2;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _articleLists3;

}
