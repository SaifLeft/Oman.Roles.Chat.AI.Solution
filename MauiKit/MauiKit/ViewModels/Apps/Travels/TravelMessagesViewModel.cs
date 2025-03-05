
namespace MauiKit.ViewModels.Travels;
public partial class TravelMessagesViewModel : BaseViewModel
{
    #region Fields
    private INavigation _navigationService;
    private Page _pageService;

    [ObservableProperty]
    private ObservableCollection<TravelMessage> _recentMessages;
    #endregion Fields

    #region Constructor
    public TravelMessagesViewModel(INavigation navigationService, Page pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;

        LoadData();

        MessageDetailCommand = new Command<TravelMessage>(OpenMessageDetail);
    }
    #endregion Constructor

    #region Methods
    void LoadData()
    {
        RecentMessages = new ObservableCollection<TravelMessage>(TravelGuideServices.Instance.GetRecentMessages);
    }

    #endregion Methods

    #region Commands
    public ICommand MessageDetailCommand { get; private set; }
    private async void OpenMessageDetail(TravelMessage selectedConversation)
    {
        await _navigationService.PushAsync(new TravelMessageDetailPage(selectedConversation));
    }

    void OnBack()
    {
        _navigationService.PopAsync();
    }

    #endregion Commands
    
}