
namespace MauiKit.ViewModels;
public partial class ListImageRoundedViewModel : ObservableObject
{
    public ObservableCollection<ListsItem> items;
    public ListImageRoundedViewModel()
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
                        Title = "Maui Store Westfield 1",
                        Description = "UX/UI mobile application design for a social network",
                        Image = AppSettings.ImageServerPath + "articles/article_01.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store Rundle Mall",
                        Description = "UX/UI mobile application design for a social network",
                        Image = AppSettings.ImageServerPath + "articles/article_02.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store Burwood 1",
                        Description = "UX/UI mobile application design for a social network",
                        Image = AppSettings.ImageServerPath + "articles/article_03.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store Westfield 2",
                        Description = "UX/UI mobile application design for a social network",
                        BackgroundColor = Color.FromArgb("#3b5998"),
                        Image = AppSettings.ImageServerPath + "articles/article_04.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store Wilga 1",
                        Description = "UX/UI mobile application design for a social network",
                        Image = AppSettings.ImageServerPath + "articles/article_05.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store Burwood 2",
                        Description = "UX/UI mobile application design for a social network",
                        Image = AppSettings.ImageServerPath + "articles/article_06.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store NorthPort",
                        Description = "UX/UI mobile application design for a social network",
                        Image = AppSettings.ImageServerPath + "articles/article_07.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store Wilga 3",
                        Description = "UX/UI mobile application design for a social network",
                        Image =  AppSettings.ImageServerPath + "articles/article_08.jpg",
                    },
                    new ListsItem
                    {
                        Title = "Maui Store Aeon Mall",
                        Description = "UX/UI mobile application design for a social network",
                        Image = AppSettings.ImageServerPath + "articles/article_09.jpg",
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
