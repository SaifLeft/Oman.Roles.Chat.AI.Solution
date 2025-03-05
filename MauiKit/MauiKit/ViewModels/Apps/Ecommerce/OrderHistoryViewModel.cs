using static MauiKit.Models.Ecommerce.TrackOrderModel;

namespace MauiKit.ViewModels.Ecommerce;

public partial class OrderHistoryViewModel : BaseViewModel
{
    public OrderHistoryViewModel()
    {
        LoadData();
    }

    [ObservableProperty]
    private List<TrackOrderModel> _orderLists;

    void LoadData()
    {
        OrderLists = new List<TrackOrderModel>(EcommerceServices.Instance.GetOrderHistories);
    }
}
