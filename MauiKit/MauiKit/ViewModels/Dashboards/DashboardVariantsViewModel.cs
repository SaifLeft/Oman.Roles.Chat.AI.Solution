
namespace MauiKit.ViewModels.Dashboards;
public partial class DashboardVariantsViewModel : ObservableObject
{
    public ObservableCollection<DashboardVariantItem> items;
    public DashboardVariantsViewModel()
    {
        CreateDashboardItemCollection();
    }

    void CreateDashboardItemCollection()
    {
        Task.Run(async () =>
        {
            IsBusy = true;
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                items = new ObservableCollection<DashboardVariantItem>
                {
                    new DashboardVariantItem
                    {
                        Title = "Photos",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#773b5998"),
                        BackgroundImage = AppSettings.ImageServerPath +  "social/gallery_10.jpg",
                        ShowBackgroundColor = false,
                        IsNotification = false,
                        Icon = IonIcons.IosCameraOutline,
                        BadgeCount = 0
                    },
                    new DashboardVariantItem
                    {
                        Title = "Gallery",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#77837bff"),
                        BackgroundImage = AppSettings.ImageServerPath +  "social/profile_image_2.jpg",
                        ShowBackgroundColor = false,
                        IsNotification = false,
                        Icon = "",
                        BadgeCount = 0
                    },
                    new DashboardVariantItem
                    {
                        Title = "Instagram",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#FA383E"),
                        BackgroundImage = "",
                        ShowBackgroundColor = false,
                        IsNotification = false,
                        Icon = IonIcons.SocialInstagramOutline,
                        BadgeCount = 0
                    },
                    
                    new DashboardVariantItem
                    {
                        Title = "Musics",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#a8d35f"),
                        BackgroundImage = "",
                        ShowBackgroundColor = true,
                        IsNotification = false,
                        Icon = IonIcons.IosMusicalNote,
                        BadgeCount = 0
                    },
                    new DashboardVariantItem
                    {
                        Title = "Shopping",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#776c88ff"),
                        BackgroundImage = AppSettings.ImageServerPath +  "ecommerce/product_item_0.jpg",
                        ShowBackgroundColor = false,
                        IsNotification = false,
                        Icon = IonIcons.Bag,
                        BadgeCount = 1
                    },
                    
                    new DashboardVariantItem
                    {
                        Title = "Globals",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#88cf62e2"),
                        BackgroundImage = AppSettings.ImageServerPath +  "news/cat-all.jpg",
                        ShowBackgroundColor = false,
                        IsNotification = false,
                        Icon = IonIcons.AndroidGlobe,
                        BadgeCount = 1
                    },
                    new DashboardVariantItem
                    {
                        Title = "Facebook",
                        Content = "MAUIKIT #AWESOME",
                        Avatar = AppSettings.ImageServerPath +  "avatars/user2.png",
                        BackgroundColor = Color.FromArgb("#1B74E4"),
                        BackgroundImage = "",
                        ShowBackgroundColor = false,
                        IsNotification = true,
                        Icon = IonIcons.SocialFacebook,
                        BadgeCount = 2
                    },
                    new DashboardVariantItem
                    {
                        Title = "Calendar",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#79b530"),
                        BackgroundImage = "",
                        ShowBackgroundColor = true,
                        IsNotification = false,
                        Icon = IonIcons.Calendar,
                        BadgeCount = 0
                    },
                    new DashboardVariantItem
                    {
                        Title = "LinkedIn",
                        Content = "MAUI Kit 2.0 preview!",
                        Avatar = AppSettings.ImageServerPath +  "avatars/user4.png",
                        BackgroundColor = Color.FromArgb("#5588fe"),
                        BackgroundImage = "",
                        ShowBackgroundColor = false,
                        IsNotification = true,
                        Icon = IonIcons.SocialLinkedin,
                        BadgeCount = 2
                    },
                    new DashboardVariantItem
                    {
                        Title = "Twitter",
                        Content = "@MAUIKit",
                        Avatar = AppSettings.ImageServerPath +  "avatars/150-2.jpg",
                        BackgroundColor = Color.FromArgb("#00bdf2"),
                        BackgroundImage = AppSettings.ImageServerPath +  "social/gallery_01.jpg",
                        ShowBackgroundColor = false,
                        IsNotification = true,
                        Icon = IonIcons.SocialTwitterOutline,
                        BadgeCount = 2
                    },
                    new DashboardVariantItem
                    {
                        Title = "Whatsapp",
                        Content = "Checkout MauiKit Now!",
                        Avatar = AppSettings.ImageServerPath +  "avatars/user3.png",
                        BackgroundColor = Color.FromArgb("#25d366"),
                        BackgroundImage = "",
                        ShowBackgroundColor = false,
                        IsNotification = true,
                        Icon = IonIcons.SocialWhatsappOutline,
                        BadgeCount = 5
                    },
                    new DashboardVariantItem
                    {
                        Title = "Pinterest",
                        Content = "MAUIKit Awesome!",
                        Avatar = AppSettings.ImageServerPath +  "avatars/user1.png",
                        BackgroundColor = Color.FromArgb("#e60023"),
                        BackgroundImage = AppSettings.ImageServerPath +  "news/cat-travel.jpg",
                        ShowBackgroundColor = false,
                        IsNotification = true,
                        Icon = IonIcons.SocialPinterest,
                        BadgeCount = 1
                    },
                    new DashboardVariantItem
                    {
                        Title = "Actions",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#ff6347"),
                        BackgroundImage = "",
                        ShowBackgroundColor = true,
                        IsNotification = false,
                        Icon = IonIcons.IosBoltOutline,
                        BadgeCount = 0
                    },
                    new DashboardVariantItem
                    {
                        Title = "Users",
                        Content = "",
                        Avatar = "",
                        BackgroundColor = Color.FromArgb("#25d366"),
                        BackgroundImage = "",
                        ShowBackgroundColor = true,
                        IsNotification = false,
                        Icon = IonIcons.AndroidContact,
                        BadgeCount = 2
                    }
                };
                IsBusy = false;

                DashboardItems = new ObservableCollection<DashboardVariantItem>(items);
                
            });
        });
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<DashboardVariantItem> _dashboardItems;
}
