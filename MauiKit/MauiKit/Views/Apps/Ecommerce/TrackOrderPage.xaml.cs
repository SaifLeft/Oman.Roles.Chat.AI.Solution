using static MauiKit.Models.Ecommerce.TrackOrderModel;

namespace MauiKit.Views.Ecommerce;

public partial class TrackOrderPage : BasePage
{
    public TrackOrderPage()
    {
        InitializeComponent();
        BindingContext = new TrackOrderViewModel();
    }
}