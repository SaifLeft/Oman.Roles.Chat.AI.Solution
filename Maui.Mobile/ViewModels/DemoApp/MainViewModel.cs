using Maui.Service;
using MauiKit.Models.MainChat;

namespace MauiKit.ViewModels;
public partial class MainViewModel : BaseViewModel, IRecipient<CultureChangeMessage>
{
    [ObservableProperty]
    bool isRTLLanguage;
    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register<CultureChangeMessage>(this);
        IsRTLLanguage = AppSettings.IsRTLLanguage;
        LoadData();
    }

    /// <summary>
    /// On received culture changed message, put your action inside MainThread
    /// </summary>
    /// <param name="message"></param>
    public void Receive(CultureChangeMessage message)
    {
        IsRTLLanguage = AppSettings.IsRTLLanguage;
        MainThread.BeginInvokeOnMainThread(() =>
        {

        });
    }
    void LoadData()
    {
        RecentTransactions = new ObservableCollection<RecentlyChatCV>(ChatService.Instance.GetTransactions);
    }

    [ObservableProperty]
    public ObservableCollection<RecentlyChatCV> _recentTransactions;
}
