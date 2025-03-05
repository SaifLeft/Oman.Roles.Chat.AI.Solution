
namespace MauiKit.ViewModels;
public partial class EwalletHomeViewModel : ObservableObject
{
    private ObservableCollection<HomeBanner> items;

    public EwalletHomeViewModel()
    {
        LoadData();
    }

    void LoadData()
    {
        RecentTransactions = new ObservableCollection<TransactionData>(EwalletServices.Instance.GetTransactions);
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    public ObservableCollection<HomeBanner> _bannerItems;

    [ObservableProperty]
    public ObservableCollection<TransactionData> _recentTransactions;
}
