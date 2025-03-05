
namespace MauiKit.ViewModels;
public partial class ListImageViewModel : ObservableObject
{
    public ObservableCollection<ListsItem> items;
    public ListImageViewModel()
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
                        BackgroundColor = Color.FromArgb("#837bff"),
                        Image = AppSettings.ImageServerPath + "articles/article_01.jpg",
                        Icon = IonIcons.SocialFacebook,
                        ItemCount = 6,
                    },
                    new ListsItem
                    {
                        Title = "Documents",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#25d366"),
                        Image = AppSettings.ImageServerPath + "articles/article_02.jpg",
                        Icon = IonIcons.Document,
                        ItemCount = 5,
                    },
                    new ListsItem
                    {
                        Title = "Messages",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#db7faa"),
                        Image = AppSettings.ImageServerPath + "articles/article_03.jpg",
                        Icon = IonIcons.Chatbubbles,
                        ItemCount = 10,
                    },
                    new ListsItem
                    {
                        Title = "Photos",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#3b5998"),
                        Image = AppSettings.ImageServerPath + "articles/article_04.jpg",
                        Icon = IonIcons.IosPhotos,
                        ItemCount = 6,
                    },
                    new ListsItem
                    {
                        Title = "Musics",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#a8d35f"),
                        Image = AppSettings.ImageServerPath + "articles/article_05.jpg",
                        Icon = IonIcons.MusicNote,
                        ItemCount = 3,
                    },
                    new ListsItem
                    {
                        Title = "Shopping",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#6c88ff"),
                        Image = AppSettings.ImageServerPath + "articles/article_06.jpg",
                        Icon = IonIcons.Bag,
                        ItemCount = 13,
                    },
                    new ListsItem
                    {
                        Title = "Globals",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#cf62e2"),
                        Image = AppSettings.ImageServerPath + "articles/article_07.jpg",
                        Icon = IonIcons.Network,
                        ItemCount = 8,
                    },
                    new ListsItem
                    {
                        Title = "Calendar",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#ec5b76"),
                        Image =  AppSettings.ImageServerPath + "articles/article_08.jpg",
                        Icon = IonIcons.IosCalendar,
                        ItemCount = 16,
                    },
                    new ListsItem
                    {
                        Title = "Settings",
                        Description = "",
                        BackgroundColor = Color.FromArgb("#7d46c2"),
                        Image = AppSettings.ImageServerPath + "articles/article_09.jpg",
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
