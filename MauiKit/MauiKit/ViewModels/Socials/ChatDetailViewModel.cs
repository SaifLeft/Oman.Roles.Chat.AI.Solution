
namespace MauiKit.ViewModels.Socials;
public partial class ChatDetailViewModel : ObservableObject
{
    private INavigation _navigationService;
    private SocialMessage _selectedConversation;
    public ChatDetailViewModel(INavigation navigationService, SocialMessage selectedConversation)
    {
        _navigationService = navigationService;
        _selectedConversation = selectedConversation;

        LoadData(_selectedConversation);
    }

    #region Methods
    void LoadData(SocialMessage _selectedConversation)
    {
        IsBusy = true;
        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                if(_selectedConversation != null)
                {
                    User = _selectedConversation.Sender;
                    Messages = new ObservableCollection<SocialMessage>(SocialServices.Instance.GetMessages(User));
                }
                
                IsBusy = false;
            });
        });
    }

    #endregion Methods

    #region Commands
    void OnBack()
    {

    }

    #endregion Commands

    #region Public Properties

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    SocialUser _user;

    [ObservableProperty]
    ObservableCollection<SocialMessage> _messages;

    #endregion Public Properties
}