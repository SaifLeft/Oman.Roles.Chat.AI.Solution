
namespace MauiKit.ViewModels.Dashboards;
public partial class DashboardTimelineViewModel : ObservableObject
{
    public ObservableCollection<TimelineData> items;
    public DashboardTimelineViewModel()
    {
        CreateDashboardItemCollection();
    }

    void CreateDashboardItemCollection()
    {
        Task.Run(async () =>
        {
            IsBusy = true;
            // await api call;
            await Task.Delay(300);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                items = new ObservableCollection<TimelineData>
                {
                    new TimelineData
                    {
                        Title = "Changed Document Owner",
                        Author = "Andrea Lang",
                        Icon = MauiKitIcons.Clock,
                        Color = Color.FromArgb("#6023FF"),
                        When = "07/15/2020 11:22AM",
                        IsInbound = true
                    },
                    new TimelineData
                    {
                        Title = "Approved Document",
                        Author = "Christina Lloyd",
                        Icon = MauiKitIcons.File,
                        Color = Color.FromArgb("#3059FE"),
                        When = "07/14/2020 11:22AM",
                        IsInbound = false
                    },
                    new TimelineData
                    {
                        Title = "Start Document Approval Process",
                        Author = "Andrea Lang",
                        Icon = MauiKitIcons.File,
                        Color = Color.FromArgb("#79b530"),
                        When = "07/13/2020 11:22AM",
                        IsInbound = true
                    },
                    new TimelineData
                    {
                        Title = "Shared Document to Andrea Lang with permission READ",
                        Author = "Christina Lloyd",
                        Icon = MauiKitIcons.Bell,
                        Color = Color.FromArgb("#ed2939"),
                        When = "07/12/2020 11:22AM",
                        IsInbound = false
                    },
                    new TimelineData
                    {
                        Title = "Shared Document to John Doe with permission READ",
                        Author = "Christina Lloyd",
                        Icon = MauiKitIcons.Bell,
                        Color = Color.FromArgb("#6023FF"),
                        When = "07/11/2020 11:22AM",
                        IsInbound = true
                    },
                    new TimelineData
                    {
                        Title = "Shared Document to David Kramer with permission READ",
                        Author = "Andrea Lang",
                        Icon = MauiKitIcons.ChatProcessing,
                        Color = Color.FromArgb("#3059FE"),
                        When = "07/10/2020 11:22AM",
                        IsInbound = false
                    },
                    new TimelineData
                    {
                        Title = "Changed Document Stage To Manager Confirmation",
                        Author = "Christina Lloyd",
                        Icon = MauiKitIcons.Bell,
                        Color = Color.FromArgb("#79b530"),
                        When = "07/09/2020 11:22AM",
                        IsInbound = true
                    },
                };
                IsBusy = false;

                TimelineItems = new ObservableCollection<TimelineData>(items);
                
            });
        });
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<TimelineData> _timelineItems;
}
