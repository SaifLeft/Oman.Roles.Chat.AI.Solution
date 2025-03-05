using static MauiKit.Models.Ecommerce.TrackOrderModel;

namespace MauiKit.Services
{
    public class EcommerceServices
    {
        static EcommerceServices _instance;
        public static EcommerceServices Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EcommerceServices();

                return _instance;
            }
        }
        
        public List<BannerModel> GetBanners
        {
            get
            {
                return new List<BannerModel>
                {
#if ANDROID || IOS
                    new BannerModel {
                        Id = 1,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_01.jpg"
                    },
                    new BannerModel {
                        Id = 2,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_02.jpg"
                    },
                    new BannerModel {
                        Id = 3,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_03.jpg"
                    },
                    new BannerModel {
                        Id = 4,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_04.jpg"
                    },
                    new BannerModel {
                        Id = 5,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_05.jpg"
                    }
#elif WINDOWS || MACCATALYST
                    new BannerModel {
                        Id = 1,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_landscape_01.jpg"
                    },
                    new BannerModel {
                        Id = 2,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_landscape_02.jpg"
                    },
                    new BannerModel {
                        Id = 3,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_landscape_03.jpg"
                    },
                    new BannerModel {
                        Id = 4,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_landscape_04.jpg"
                    },
                    new BannerModel {
                        Id = 5,
                        ImageUrl = AppSettings.ImageServerPath +  "ecommerce/banner_landscape_05.jpg"
                    }
#endif
                };
            }
        }

        public List<CategoryModel> GetCategories
        {
            get
            {
                return new List<CategoryModel>
                {
                    new CategoryModel { CategoryID = 1, CategoryName = "Men", Icon = MauiKitIcons.ShoeFormal },
                    new CategoryModel { CategoryID = 2, CategoryName = "Women", Icon = MauiKitIcons.ShoeHeel },
                    new CategoryModel { CategoryID = 3, CategoryName = "Devices", Icon = MauiKitIcons.Laptop },
                    new CategoryModel { CategoryID = 4, CategoryName = "Gadgets", Icon = MauiKitIcons.Headphones },
                    new CategoryModel { CategoryID = 5, CategoryName = "Games", Icon = MauiKitIcons.XboxController },
                    new CategoryModel { CategoryID = 6, CategoryName = "Speakers", Icon = MauiKitIcons.Speaker },
                    new CategoryModel { CategoryID = 7, CategoryName = "Cameras", Icon = MauiKitIcons.Camera }
                };
            }
        }

        public List<ProductDetail> GetAllProducts
        {
            get
            {
                return new List<ProductDetail>
                {
                    new ProductDetail {
                        Id = 1,
                        Name = "BeoPlay Speaker",
                        BrandName = "Bang and Olufsen",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_0.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image2.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image3.png"},
                        Price = "$755",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail  {
                        Id = 2,
                        Name = "Leather Wristwatch",
                        BrandName = "Tag Heuer",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_1.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image3.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image1.png"},
                        Price = "$450",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 3,
                        Name = "Smart Bluetooth Speaker",
                        BrandName = "Google LLC",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_2.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image1.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image2.png"},
                        Price = "$900",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 4,
                        Name = "Smart Bluetooth Earphone",
                        BrandName = "Smart Inc",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_3.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image7.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image8.png"},
                        Price = "$1200",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 5,
                        Name = "B&o Desk Lamp",
                        BrandName = "Bang and Olufsen",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_4.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image8.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image9.png"},
                        Price = "$450",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 6,
                        Name = "BeoPlay Stand Speaker",
                        BrandName = "Bang and Olufsen",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product_item_5.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image7.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image9.png"},
                        Price = "$120",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 7,
                        Name = "Airpods",
                        BrandName = "B&o Phone Case",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-5.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image8.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image7.png"},
                        Price = "$100",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 8,
                        Name = "BeoPlay Speaker",
                        BrandName = "Bang and Olufsen",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-6.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image2.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image3.png"},
                        Price = "$755",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 9,
                        Name = "Leather Wristwatch",
                        BrandName = "Tag Heuer",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-7.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image3.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image1.png"},
                        Price = "$450",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 10,
                        Name = "Smart Bluetooth Speaker",
                        BrandName = "Google LLC",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-8.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image1.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image2.png"},
                        Price = "$900",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 11,
                        Name = "Smart Bluetooth Earphone",
                        BrandName = "Smart Inc",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-9.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image7.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image8.png"},
                        Price = "$1200",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    },
                    new ProductDetail {
                        Id = 12,
                        Name = "B&o Desk Lamp",
                        BrandName = "Bang and Olufsen",
                        ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-10.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image8.png", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/Image9.png"},
                        Price = "$450",
                        Sizes = new List<string>() {"S", "M", "L"},
                        Reviews = new List<ReviewModel>() {
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "October 8th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 4,
                                Date = "August 25th 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                                Name = "Samuel Smith",
                                Review = "Wonderful jean, perfect gift for my girl for our anniversary!",
                                Rating = 4,
                                Date = "November 2nd 2021"
                            },
                            new ReviewModel() {
                                ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                                Name = "Beth Aida",
                                Review = "I love this, paired it with a nice blouse and all eyes on me.",
                                Rating = 5,
                                Date = "Feb 15th 2022"
                            }
                        },
                        ColorLists = new List<Color>() { Color.FromArgb("#42337D"), Color.FromArgb("#7D427D"), Color.FromArgb("#337D42") },
                        Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                    }
                };
            }
        }

        public ProductDetail  GetProductDetail
        {
            get 
            {
                return new ProductDetail
                {
                    Id = 1,
                    Name = "BeoPlay Speaker",
                    BrandName = "Bang and Olufsen",
#if ANDROID || IOS
                    ImageUrls = new List<string>() 
                    { 
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-1.jpg", 
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-2.jpg", 
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-3.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-4.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-5.jpg"
                    },
#elif WINDOWS || MACCATALYST
                    ImageUrls = new List<string>() 
                    { 
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-landscape-1.jpg", 
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-landscape-2.jpg", 
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-landscape-3.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-landscape-4.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-detail-landscape-5.jpg"
                    },
#endif
                    Price = "$755",
                    Sizes = new List<string>() { "S", "M", "L", "XL", "XXL" },
                    Reviews = new List<ReviewModel>() {
                        new ReviewModel() {
                            ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                            Name = "Samuel Smith",
                            Review = "Wonderful jean, perfect gift for my girl for our anniversary! I love this, paired it with a nice blouse and all eyes on me.",
                            Rating = 4.3,
                            Date = "October 8th 2021"
                        },
                        new ReviewModel() {
                            ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                            Name = "Beth Aida",
                            Review = "I love this, paired it with a nice blouse and all eyes on me. Wonderful jean, perfect gift for my girl for our anniversary!",
                            Rating = 4.0,
                            Date = "August 25th 2021"
                        },
                        new ReviewModel() {
                            ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user1.png",
                            Name = "Samuel Smith",
                            Review = "Wonderful jean, perfect gift for my girl for our anniversary!  I love this, paired it with a nice blouse and all eyes on me.",
                            Rating = 4.5,
                            Date = "November 2nd 2021"
                        },
                        new ReviewModel() {
                            ImageUrl = AppSettings.ImageServerPath +  "ecommerce/users/user2.png",
                            Name = "Beth Aida",
                            Review = "I love this, paired it with a nice blouse and all eyes on me. Wonderful jean, perfect gift for my girl for our anniversary!",
                            Rating = 5.0,
                            Date = "Feb 15th 2022"
                        }
                    },
                    ColorLists = new List<Color>() { Color.FromArgb("#1A73E8"), Color.FromArgb("#F7B548"), Color.FromArgb("#FF392B"), Color.FromArgb("#00C569"), Color.FromArgb("#2B0B98") },
                    Details = "Nike Dri-FIT is a polyester fabric designed to help you keep dry so you can more comfortably work harder, longer."
                };
            }
        }
        
        public List<ProductListModel> GetBestSellers
        {
            get 
            {
                return new List<ProductListModel>
            {
                new ProductListModel {
                    Name = "BeoPlay Speaker",
                    BrandName = "Bang and Olufsen",
                    Price = "$755",
                    ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-1.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-2.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-3.jpg" }
                },
                new ProductListModel {
                    Name = "Leather Wristwatch",
                    BrandName = "Tag Heuer",
                    Price = "$450",
                    ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-2.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-3.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-1.jpg" }
                },
                new ProductListModel {
                    Name = "Smart Bluetooth Speaker",
                    BrandName = "Google LLC",
                    Price = "$900",
                    ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-6.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-1.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-3.jpg" }
                },
                new ProductListModel {
                    Name = "Smart Bluetooth Earphone",
                    BrandName = "Smart Inc",
                    Price = "$1200",
                    ImageUrls = new List<string>() { "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-4.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-5.jpg", "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-6.jpg" }
                }
            };
            }
        }

        public List<ProductListModel> GetRecommendeds
        {
            get
            {
                return new List<ProductListModel>
            {
                new ProductListModel {
                    Name = "BeoPlay Speaker",
                    BrandName = "Bang and Olufsen",
                    Price = "$755",
                    ImageUrls = new List<string>()
                    {
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-4.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-5.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-6.jpg"
                    }
                },
                new ProductListModel {
                    Name = "Leather Wristwatch",
                    BrandName = "Tag Heuer",
                    Price = "$450",
                    ImageUrls = new List<string>()
                    {
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-7.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-8.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-9.jpg"
                    }
                },
                new ProductListModel {
                    Name = "Smart Bluetooth Speaker",
                    BrandName = "Google LLC",
                    Price = "$900",
                    ImageUrls = new List<string>()
                    {
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-10.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-11.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-12.jpg"
                    }
                },
                new ProductListModel {
                    Name = "Smart Bluetooth Earphone",
                    BrandName = "Smart Inc",
                    Price = "$1200",
                    ImageUrls = new List<string>()
                    {
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-5.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-6.jpg",
                        "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/ecommerce/product-7.jpg"
                    }
                }
            };
            }
        }

        public List<BrandListModel> GetFeaturedBrands
        {
            get
            {
                return new List<BrandListModel>
            {
                new BrandListModel {
                    BrandName = "B&o",
                    Details = "5693 Products",
                    ImageUrl = AppSettings.ImageServerPath +  "ecommerce/brands/Icon_Bo.png"
                },
                new BrandListModel {
                    BrandName = "Beats",
                    Details = "1124 Products",
                    ImageUrl = AppSettings.ImageServerPath +  "ecommerce/brands/beats.png"
                },
                new BrandListModel {
                    BrandName = "Apple Inc",
                    Details = "5693 Products",
                    ImageUrl = AppSettings.ImageServerPath +  "ecommerce/brands/Icon_Apple.png"
                },
                new BrandListModel {
                    BrandName = "Adidas",
                    Details = "453 Products",
                    ImageUrl = AppSettings.ImageServerPath +  "ecommerce/brands/01.png"
                },
                new BrandListModel {
                    BrandName = "Nike",
                    Details = "6368 Products",
                    ImageUrl = AppSettings.ImageServerPath +  "ecommerce/brands/04.png"
                }
            };
            }
        }

        public List<TrackOrderModel> GetOrderHistories
        {
            get
            {
                return new List<TrackOrderModel>
            {
                new TrackOrderModel("Oct 10, 2022", new List<Track>
                {
                    new Track
                    {
                        OrderId = "OD - 54548786 - N",
                        Price = "$4500",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_6.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_7.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_8.png"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_9.png"}
                        }
                    }
                }),
                new TrackOrderModel("Oct 18, 2022", new List<Track>
                {
                    new Track
                    {
                        OrderId = "OD - 68349764 - N",
                        Price = "$500",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_5.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_6.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_7.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_8.jpg"}
                        }
                    },
                    new Track
                    {
                        OrderId = "OD - 64327854 - N",
                        Price = "$700",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_8.png"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_9.png"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_2.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_3.jpg"}
                        }
                    }
                }),
                new TrackOrderModel("Nov 01, 2022", new List<Track>
                {
                    new Track
                    {
                        OrderId = "OD - 87544456 - N",
                        Price = "$1500",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-4.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-5.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-6.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-7.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-8.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-9.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-10.jpg"},
                        }
                    },
                    new Track
                    {
                        OrderId = "OD - 76544532 - N",
                        Price = "$2700",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-11.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-12.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-1.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-2.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-3.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-4.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-5.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-6.jpg"},
                        }
                    }
                }),
                new TrackOrderModel("Nov 15, 2022", new List<Track>
                {
                    new Track
                    {
                        OrderId = "OD - 87656548 - N",
                        Price = "$4500",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-7.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-8.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-9.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-1.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-2.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product-3.jpg"},
                        }
                    }
                }),
                new TrackOrderModel("Nov 27, 2022", new List<Track>
                {
                    new Track
                    {
                        OrderId = "OD - 54627895 - N",
                        Price = "$4500",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_7.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_0.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_1.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_2.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_3.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_4.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_5.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_6.jpg"},
                        }
                    }
                }),
                new TrackOrderModel("Dec 02, 2022", new List<Track>
                {
                    new Track
                    {
                        OrderId = "OD - 755635653 - N",
                        Price = "$4500",
                        Status = "Delivered",
                        Images= new List<ImageList>()
                        {
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_1.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_2.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_3.jpg"},
                            new ImageList(){ImageUrl = AppSettings.ImageServerPath +  "ecommerce/product_item_4.jpg"},
                        }
                    }
                })
            };
            }
        }

        public List<DeliveryStepsModel> GetDeliverySteps
        {
            get
            {
                return new List<DeliveryStepsModel>
                {
                    new DeliveryStepsModel { Id = 1, DateMonth = "20 Nov", IsComplete = true, Time = "12:00", Name = "Order placed", Location = "WA, Victoria" },
                    new DeliveryStepsModel { Id = 2, DateMonth = "20 Nov", IsComplete = true, Time = "12:15", Name = "Order confirmed", Location = "WA, Victoria" },
                    new DeliveryStepsModel { Id = 3, DateMonth = "20 Nov", IsComplete = true, Time = "14:00", Name = "Order prepared", Location = "WA, Victoria" },
                    new DeliveryStepsModel { Id = 4, DateMonth = "21 Nov", IsComplete = false, Time = "03:00", Name = "Shipping out", Location = "WA, Victoria" },
                    new DeliveryStepsModel { Id = 5, DateMonth = "21 Nov", IsComplete = false, Time = "06:00", Name = "Delivered", Location = "WA, Victoria" }
                };
            }
        }
    }
}
