
namespace MauiKit.ViewModels;
public partial class MobileTopupViewModel : ObservableObject
{
    public MobileTopupViewModel()
    {
        LoadData();
    }

    void LoadData()
    {
        ContactLists = new ObservableCollection<WalletContact>(EwalletServices.Instance.GetContacts);
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    public ObservableCollection<WalletContact> _contactLists;
}
