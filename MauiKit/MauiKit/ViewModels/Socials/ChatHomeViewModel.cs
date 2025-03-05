
namespace MauiKit.ViewModels.Socials;
public partial class ChatHomeViewModel : ObservableObject
{
    private INavigation _navigationService;
    private Page _pageService;
    
    public ChatHomeViewModel(INavigation navigationService, Page pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;

        LoadData();
    }

    //public ICommand DetailCommand => new Command<object>(OnNavigate);

    #region Methods
    void LoadData()
    {
        IsBusy = true;
        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                Users = new ObservableCollection<SocialUser>(SocialServices.Instance.GetUsers());
                RecentChat = new ObservableCollection<SocialMessage>(SocialServices.Instance.GetChats());
                
                IsBusy = false;
            });
        });
    }

    #endregion Methods

    #region Commands

    [RelayCommand]
    private async void OpenConversation(SocialMessage selectedConversation)
    {
        await _navigationService.PushAsync(new ChatDetailPage(selectedConversation));
    }

    void OnBack()
    {

    }

    #endregion Commands
    

    #region Public Properties

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    ObservableCollection<SocialUser> _users;

    [ObservableProperty]
    ObservableCollection<SocialMessage> _recentChat;

    #endregion Public Properties
}