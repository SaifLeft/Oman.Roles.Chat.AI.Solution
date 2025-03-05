namespace MauiKit.ViewModels.Dashboards;
public partial class DashboardCardViewModel : ObservableObject
{
    public DashboardCardViewModel()
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
                DashboardItems = new ObservableCollection<ArticleData>(ArticleServices.Instance.GetArticles());
                
                IsBusy = false;
            });
        });
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<ArticleData> _dashboardItems;

}
