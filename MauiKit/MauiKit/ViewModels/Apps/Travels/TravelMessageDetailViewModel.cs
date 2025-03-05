
namespace MauiKit.ViewModels.Travels;
public partial class TravelMessageDetailViewModel : BaseViewModel
{
    [ObservableProperty]
    private TravelMessage _selectedConversation;

    [ObservableProperty]
    private TravelUser _user;

    [ObservableProperty]
    private ObservableCollection<TravelMessage> _messages;
    public TravelMessageDetailViewModel(TravelMessage selectedConversation)
    {
        _selectedConversation = selectedConversation;

        LoadData(_selectedConversation);
    }

    #region Methods
    void LoadData(TravelMessage _selectedConversation)
    {
        User = _selectedConversation.Sender;
        Messages = new ObservableCollection<TravelMessage>(TravelGuideServices.Instance.GetConversationDetail(User));
    }

    #endregion Methods

}