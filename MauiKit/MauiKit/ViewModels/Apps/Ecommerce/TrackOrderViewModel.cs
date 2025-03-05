using static MauiKit.Models.Ecommerce.TrackOrderModel;

namespace MauiKit.ViewModels.Ecommerce;

public partial class TrackOrderViewModel : BaseViewModel
{
    [ObservableProperty]
    private List<DeliveryStepsModel> _trackStatusData;
    
    public TrackOrderViewModel()
    {
        LoadData();
    }

    void LoadData()
    {
        TrackStatusData = new List<DeliveryStepsModel>(EcommerceServices.Instance.GetDeliverySteps);
    }

}
