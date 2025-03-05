
namespace MauiKit.ViewModels.Dashboards;
public partial class DashboardCarouselViewModel : ObservableObject
{
    private ObservableCollection<HomeBanner> items;

    public DashboardCarouselViewModel()
    {
        LoadData();
    }

    void LoadData()
    {
        Task.Run(async () =>
        {
            IsBusy = true;
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                items = new ObservableCollection<HomeBanner>
                {
                    new HomeBanner
                    {
                        Title = "The Happy Times",
                        Icon = IonIcons.Images,
                        //BackgroundColor = Color.FromArgb("#d81b60"),
                        BackgroundColor = Color.FromArgb("#E14D2A"),
                        Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                        BackgroundImage = "bg_abs.jpg"
                    },
                    new HomeBanner
                    {
                        Title = "A Look Inside The Work",
                        Icon = IonIcons.Videocamera,
                        BackgroundColor = Color.FromArgb("#2e7d32"),
                        Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                        BackgroundImage = "bg_trans.png"
                    },
                    new HomeBanner
                    {
                        Title = "Simple Acts of Kindness",
                        Icon = IonIcons.IosAlbums,
                        //BackgroundColor = Color.FromArgb("#56329A"),
                        BackgroundColor = Color.FromArgb("#F94892"),
                        Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                        BackgroundImage = "bg_abs.jpg"
                    },
                    new HomeBanner
                    {
                        Title = "Latest Style Ideas For Designer",
                        Icon = IonIcons.GearA,
                        BackgroundColor = Color.FromArgb("#4B436E"),
                        Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                        BackgroundImage = "bg_trans.png"
                    },
                    new HomeBanner
                    {
                        Title = "Clean and Colorful",
                        Icon = IonIcons.AndroidApps,
                        BackgroundColor = Color.FromArgb("#1e88e5"),
                        Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur.",
                        BackgroundImage = "bg_tablet.png"
                    }
                };

                BannerItems = new ObservableCollection<HomeBanner>(items);

                //Create Dashboard grid icons
                var dashboardIcons = new List<NavigationMenuItem>
                {
                    new NavigationMenuItem()
                    {
                        Title = "Calendar",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.IosCalendar,
                            IconColor = Color.FromArgb("#023f88")
                        },
                        TargetType = typeof(MainPage)
                    },
                    
                    new NavigationMenuItem()
                    {
                        Title = "Documents",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.Document,
                            IconColor = Color.FromArgb("#79b530")
                        },
                        TargetType = typeof(MainPage)
                    },
                    new NavigationMenuItem()
                    {
                        Title = "Gallery",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.IosCamera,
                            IconColor = Color.FromArgb("#FFfd50c8")
                        },
                        TargetType = typeof(MainPage)
                    },
                    new NavigationMenuItem()
                    {
                        Title = "Messages",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.Chatbubbles,
                            IconColor = Color.FromArgb("#ffb3cc00")
                        },
                        TargetType = typeof(MainPage)
                    },
                    new NavigationMenuItem()
                    {
                        Title = "Musics",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.MusicNote,
                            IconColor = Color.FromArgb("#ed2939")
                        },
                        TargetType = typeof(MainPage)
                    },
                    
                    new NavigationMenuItem()
                    {
                        Title = "News",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.SocialDesignernews,
                            IconColor = Color.FromArgb("#FF7d46c2")
                        },
                        TargetType = typeof(MainPage)
                    },
                    new NavigationMenuItem()
                    {
                        Title = "Settings",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.Settings,
                            IconColor = Color.FromArgb("#00bdf2")
                        },
                        TargetType = typeof(MainPage)
                    },
                    new NavigationMenuItem()
                    {
                        Title = "Shopping",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.IosCart,
                            IconColor = Color.FromArgb("#FF25d366")
                        },
                        TargetType = typeof(MainPage)
                    },
                    new NavigationMenuItem()
                    {
                        Title = "Users",
                        FontImageIcon = new NavigationMenuIconItem()
                        {
                            Icon = IonIcons.AndroidContacts,
                            IconColor = Color.FromArgb("#ff6347")
                        },
                        TargetType = typeof(MainPage)
                    }
                };

                var dashboardIconCollection = new ObservableCollection<NavigationMenuItem>();
                foreach (var dashboardIcon in dashboardIcons)
                {
                    dashboardIconCollection.Add(dashboardIcon);
                }
                NavigationItems = dashboardIconCollection;

                IsBusy = false;
            });
        });
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    public ObservableCollection<HomeBanner> _bannerItems;

    [ObservableProperty]
    public ObservableCollection<NavigationMenuItem> _navigationItems;
}
