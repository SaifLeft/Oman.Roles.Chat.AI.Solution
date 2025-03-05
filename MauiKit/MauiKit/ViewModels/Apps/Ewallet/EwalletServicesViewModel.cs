namespace MauiKit.ViewModels;
public partial class EwalletServicesViewModel : ObservableObject
{
    public EwalletServicesViewModel()
    {
        LoadData();
    }

    void LoadData()
    {
        AllServices = new ObservableCollection<ServiceCategoryGroup>(EwalletServices.Instance.GetAllServices);
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    public ObservableCollection<ServiceCategoryGroup> _allServices;
}
