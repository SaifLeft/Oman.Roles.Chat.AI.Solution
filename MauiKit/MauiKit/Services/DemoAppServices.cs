
namespace MauiKit.Services;
public class DemoAppServices
{
    static DemoAppServices _instance;

    public static DemoAppServices Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DemoAppServices();

            return _instance;
        }
    }

    public static readonly Random Random = new Random();

    public List<Color> Colors { get; } = new List<Color>()
    {
        Color.FromArgb("#7644ad"),
        Color.FromArgb("#d54381"),
        Color.FromArgb("#E88F1A"),
        Color.FromArgb("#8010E0"),
        Color.FromArgb("#7ed321"),
        Color.FromArgb("#ff4a4a"),
        Color.FromArgb("#ff844a"),
        Color.FromArgb("#4acaff"),
        Color.FromArgb("#567cd7"),
        Color.FromArgb("#523ee8"),
        Color.FromArgb("#35c659"),
        Color.FromArgb("#d483fc")
    };

    public List<HomeTransactionData> GetUserTransactions
    {
        get
        {
            return new List<HomeTransactionData>
            {
                new HomeTransactionData
                {
                    ImageIcon = AppSettings.ImageServerPath +  "avatars/150-1.jpg",
                    Title = "Antoni Whitney",
                    Subtitle = "52540 USD",
                    Status = "Online",
                    Amount = 135.50,
                    IsCredited = true
                },
                new HomeTransactionData
                {
                    ImageIcon = AppSettings.ImageServerPath +  "avatars/150-2.jpg",
                    Title = "Alissia Shah",
                    Subtitle = "52540 USD",
                    Status = "Online",
                    Amount = 135.50,
                    IsCredited = false
                },
                new HomeTransactionData
                {
                    ImageIcon = AppSettings.ImageServerPath +  "avatars/150-3.jpg",
                    Title = "Komal Orr",
                    Subtitle = "52540 USD",
                    Status = "Online",
                    Amount = 135.50,
                    IsCredited = true
                },
                new HomeTransactionData
                {
                    ImageIcon = AppSettings.ImageServerPath +  "avatars/150-5.jpg",
                    Title = "Eathan Sheridan",
                    Subtitle = "52540 USD",
                    Status = "Online",
                    Amount = 135.50,
                    IsCredited = true
                },
                new HomeTransactionData
                {
                    ImageIcon = AppSettings.ImageServerPath +  "avatars/150-6.jpg",
                    Title = "Cecily Trujillo",
                    Subtitle = "52540 USD",
                    Status = "Online",
                    Amount = 135.50,
                    IsCredited = false
                },
                new HomeTransactionData
                {
                    ImageIcon = AppSettings.ImageServerPath +  "avatars/150-7.jpg",
                    Title = "Alaya Cordova",
                    Subtitle = "52540 USD",
                    Status = "Online",
                    Amount = 135.50,
                    IsCredited = true
                }
            };
        }
    }

    public List<NewAnnouncementData> GetNewAnnouncements
    {
        get
        {
            return new List<NewAnnouncementData>
            {
                new NewAnnouncementData
                {
                    CoverImage = AppSettings.ImageServerPath +  "articles/article_01.jpg",
                    Title = "Share application and Earning",
                    Content = "Get $10 instant as reward while your friend or invited member join MauiKit"
                },
                new NewAnnouncementData
                {
                    CoverImage = AppSettings.ImageServerPath +  "articles/article_02.jpg",
                    Title = "Earn Rewards of 50% more",
                    Content = "Get $10 instant as reward while your friend or invited member join MauiKit"
                }
            };
        }
    }

    public List<MemberData> GetMembers
    {
        get
        {
            return new List<MemberData>
            {
                new MemberData
                {
                    FullName = "Alaya Cordova",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-1.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Cecily Trujillo",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-2.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Eathan Sheridan",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-3.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Komal Orr",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-4.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Sariba Abood",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-5.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Justin O'Moore",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-6.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Alissia Shah",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-7.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Antoni Whitney",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-8.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                },
                new MemberData
                {
                    FullName = "Jaime Zuniga",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-9.jpg",
                    Position = "UIUX Designer",
                    PhoneNumber = "324-157-3235"
                }
            };
        }
    }

    public List<BannerData> GetBannerItems
    {
        get
        {
            return new List<BannerData>
            {
                new BannerData
                {
                    Title = "The Happy Times",
                    Icon = IonIcons.Images,
                    BackgroundColor = Color.FromArgb("#f13421"),
                    Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                    Status = "10 joined",
                    BackgroundImage = "bg_abs.jpg"
                },
                new BannerData
                {
                    Title = "A Look Inside The Work",
                    Icon = IonIcons.Videocamera,
                    BackgroundColor = Color.FromArgb("#ef5b00"),
                    Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                    Status = "12 joined",
                    BackgroundImage = "bg_trans.png"
                },
                new BannerData
                {
                    Title = "Simple Acts of Kindness",
                    Icon = IonIcons.Coffee,
                    BackgroundColor = Color.FromArgb("#1483cd"),
                    Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                    Status = "15 joined",
                    BackgroundImage = "bg_abs.jpg"
                },
                new BannerData
                {
                    Title = "Cutting-edge Design Ideas",
                    Icon = IonIcons.Nuclear,
                    BackgroundColor = Color.FromArgb("#ff256c"),
                    Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                    Status = "20 joined",
                    BackgroundImage = "bg_trans.png"
                },
                new BannerData
                {
                    Title = "Clean and Colorful",
                    Icon = IonIcons.AndroidColorPalette,
                    BackgroundColor = Color.FromArgb("#9987ff"),
                    Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                    Status = "28 joined",
                    BackgroundImage = "bg_tablet.png"
                }
            };
        }
    }

    public List<WalletContact> GetContacts
    {
        get
        {
            return new List<WalletContact>
            {
                new WalletContact
                {
                    Name = "Alaya Cordova",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-1.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Cecily Trujillo",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-2.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Eathan Sheridan",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-3.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Komal Orr",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-4.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Sariba Abood",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-5.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Justin O'Moore",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-6.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Alissia Shah",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-7.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Antoni Whitney",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-8.jpg",
                    PhoneNumber = "324-157-3235"
                },
                new WalletContact
                {
                    Name = "Jaime Zuniga",
                    Avatar = AppSettings.ImageServerPath +  "avatars/150-9.jpg",
                    PhoneNumber = "324-157-3235"
                }
            };
        }
    }

    public ObservableCollection<ServiceCategoryGroup> GetAllServices
    {
        get
        {
            return new ObservableCollection<ServiceCategoryGroup>
            {
                new ServiceCategoryGroup ("Bill", new List<ServiceCategory>
                {
                    new ServiceCategory
                    {
                        Name = "Electricity",
                        Icon = MauiKitIcons.LightbulbOn,
                        IconColor = Colors[Random.Next(3)],
                    },
                    new ServiceCategory
                    {
                        Name = "Water",
                        Icon = MauiKitIcons.Water,
                        IconColor = Colors[Random.Next(3)],
                    },
                    new ServiceCategory
                    {
                        Name = "Internet",
                        Icon = MauiKitIcons.Wifi,
                        IconColor = Colors[Random.Next(3)],
                    },
                    new ServiceCategory
                    {
                        Name = "Television",
                        Icon = MauiKitIcons.Television,
                        IconColor = Colors[Random.Next(3)],
                    },
                    new ServiceCategory
                    {
                        Name = "E-Wallet",
                        Icon = MauiKitIcons.Wallet,
                        IconColor = Colors[Random.Next(3)],
                    },
                    new ServiceCategory
                    {
                        Name = "Games",
                        Icon = MauiKitIcons.Gamepad,
                        IconColor = Colors[Random.Next(3)],
                    },
                    new ServiceCategory
                    {
                        Name = "Merchant",
                        Icon = MauiKitIcons.Cart,
                        IconColor = Colors[Random.Next(3)],
                    },
                    new ServiceCategory
                    {
                        Name = "Installment",
                        Icon = MauiKitIcons.Card,
                        IconColor = Colors[Random.Next(3)],
                    }
                }),
                new ServiceCategoryGroup ("Insurance", new List<ServiceCategory>
                {
                    new ServiceCategory
                    {
                        Name = "Health",
                        Icon = MauiKitIcons.Security,
                        IconColor = Colors[Random.Next(4)],
                    },
                    new ServiceCategory
                    {
                        Name = "Mobile",
                        Icon = MauiKitIcons.Cellphone,
                        IconColor = Colors[Random.Next(4)],
                    },
                    new ServiceCategory
                    {
                        Name = "Motor",
                        Icon = MauiKitIcons.Motorbike,
                        IconColor = Colors[Random.Next(4)],
                    },
                    new ServiceCategory
                    {
                        Name = "Car",
                        Icon = MauiKitIcons.Car,
                        IconColor = Colors[Random.Next(4)],
                    }
                }),
                new ServiceCategoryGroup ("Option", new List<ServiceCategory>
                {
                    new ServiceCategory
                    {
                        Name = "Assurance",
                        Icon = MauiKitIcons.Briefcase,
                        IconColor = Colors[Random.Next(5)],
                    },
                    new ServiceCategory
                    {
                        Name = "Shopping",
                        Icon = MauiKitIcons.Shopping,
                        IconColor = Colors[Random.Next(5)],
                    },
                    new ServiceCategory
                    {
                        Name = "Deal",
                        Icon = MauiKitIcons.Sale,
                        IconColor = Colors[Random.Next(5)],
                    },
                    new ServiceCategory
                    {
                        Name = "Invest",
                        Icon = MauiKitIcons.ProgressCheck,
                        IconColor = Colors[Random.Next(5)],
                    }
                })
            };
        }
    }

    public List<CardData> GetMyCards
    {
        get
        {
            return new List<CardData>
            {
                new CardData
                {
                    CardType = "CREDIT CARD",
                    Number = "****  ****  ****  5838",
                    Name = "Alex Wilson",
                    Expiry = "08/25",
                    CardCvv = 846,
                    BackgroundImage = "abs_bg.png",
                    BackgroundGradientStart = Color.FromArgb("#BF3F0041"),
                    BackgroundGradientEnd = Color.FromArgb("#012E8B"),
                    CardTypeIcon = "master_card.png"
                },
                new CardData
                {
                    CardType = "DEBIT CARD",
                    Number = "****  ****  ****  0743",
                    Name = "Alex Wilson",
                    Expiry = "05/23",
                    CardCvv = 543,
                    BackgroundImage = "bg_tablet.png",
                    BackgroundGradientStart = Color.FromArgb("#af4aff"),
                    BackgroundGradientEnd = Color.FromArgb("#3e5aff"),
                    CardTypeIcon = "visa.png"
                },
                new CardData
                {
                    CardType = "CREDIT CARD",
                    Number = "****  ****  ****  0629",
                    Name = "Alex Wilson",
                    Expiry = "06/26",
                    CardCvv = 346,
                    BackgroundImage = "bg_trans.png",
                    BackgroundGradientStart = Color.FromArgb("#d54381"),
                    BackgroundGradientEnd = Color.FromArgb("#7644ad"),
                    CardTypeIcon = "master_card.png"
                },
            };
        }
    }
}
