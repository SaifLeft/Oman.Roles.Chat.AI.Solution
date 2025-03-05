
namespace MauiKit.ViewModels.Dashboards;
public partial class DashboardGridViewModel : ObservableObject
{
    public ObservableCollection<DashboardGridItem> items;
    public DashboardGridViewModel()
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
                items = new ObservableCollection<DashboardGridItem>
                {
                    new DashboardGridItem
                    {
                        Title = "Facebook",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#023f88"),
                        BackgroundImage = "",
                        Icon = IonIcons.SocialFacebook,
                        ItemCount = 6,
                    },
                    new DashboardGridItem
                    {
                        Title = "Documents",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#79b530"),
                        BackgroundImage = "",
                        Icon = IonIcons.Document,
                        ItemCount = 5,
                    },
                    new DashboardGridItem
                    {
                        Title = "Messages",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#FFfd50c8"),
                        BackgroundImage = "",
                        Icon = IonIcons.Chatbubbles,
                        ItemCount = 10,
                    },
                    new DashboardGridItem
                    {
                        Title = "Photos",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#ffb3cc00"),
                        BackgroundImage = "",
                        Icon = IonIcons.IosPhotos,
                        ItemCount = 6,
                    },
                    new DashboardGridItem
                    {
                        Title = "Musics",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#ed2939"),
                        BackgroundImage = "",
                        Icon = IonIcons.MusicNote,
                        ItemCount = 3,
                    },
                    new DashboardGridItem
                    {
                        Title = "Shopping",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#FF7d46c2"),
                        BackgroundImage = "",
                        Icon = IonIcons.Bag,
                        ItemCount = 13,
                    },
                    new DashboardGridItem
                    {
                        Title = "News",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#00bdf2"),
                        BackgroundImage = "",
                        Icon = IonIcons.Network,
                        ItemCount = 8,
                    },
                    new DashboardGridItem
                    {
                        Title = "Calendar",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#FF25d366"),
                        BackgroundImage = "",
                        Icon = IonIcons.IosCalendar,
                        ItemCount = 16,
                    },
                    new DashboardGridItem
                    {
                        Title = "Settings",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#ff6347"),
                        BackgroundImage = "",
                        Icon = IonIcons.Settings,
                        ItemCount = 5,
                    }
                };
                IsBusy = false;

                DashboardItems = new ObservableCollection<DashboardGridItem>(items);
                
            });
        });
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<DashboardGridItem> _dashboardItems;
}
