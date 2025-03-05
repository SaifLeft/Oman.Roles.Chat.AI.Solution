
namespace MauiKit.ViewModels;
public partial class ListIconViewModel : ObservableObject
{
    public ObservableCollection<ListsItem> items;
    public ListIconViewModel()
    {
        CreateDashboardItemCollection();
    }

    void CreateDashboardItemCollection()
    {
        Task.Run(async () =>
        {
            IsBusy = true;
            // await api call;
            await Task.Delay(500);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                items = new ObservableCollection<ListsItem>
                {
                    new ListsItem
                    {
                        Title = "Facebook",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#023f88"),
                        Icon = IonIcons.SocialFacebook,
                        ItemCount = 6,
                    },
                    new ListsItem
                    {
                        Title = "Documents",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#79b530"),
                        Icon = IonIcons.Document,
                        ItemCount = 5,
                    },
                    new ListsItem
                    {
                        Title = "Messages",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#FFfd50c8"),
                        Icon = IonIcons.Chatbubbles,
                        ItemCount = 10,
                    },
                    new ListsItem
                    {
                        Title = "Photos",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#ffb3cc00"),
                        Icon = IonIcons.IosPhotos,
                        ItemCount = 6,
                    },
                    new ListsItem
                    {
                        Title = "Musics",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#ed2939"),
                        Icon = IonIcons.MusicNote,
                        ItemCount = 3,
                    },
                    new ListsItem
                    {
                        Title = "Shopping",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#FF7d46c2"),
                        Icon = IonIcons.Bag,
                        ItemCount = 13,
                    },
                    new ListsItem
                    {
                        Title = "Globals",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#00bdf2"),
                        Icon = IonIcons.Network,
                        ItemCount = 8,
                    },
                    new ListsItem
                    {
                        Title = "Calendar",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#FF25d366"),
                        Icon = IonIcons.IosCalendar,
                        ItemCount = 16,
                    },
                    new ListsItem
                    {
                        Title = "Settings",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#ff6347"),
                        Icon = IonIcons.Settings,
                        ItemCount = 5,
                    }
                };
                IsBusy = false;

                ListItems = new ObservableCollection<ListsItem>(items);
                
            });
        });
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<ListsItem> _listItems;
}
