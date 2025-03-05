namespace MauiKit.Views.Ecommerce;

public partial class OrderHistoryPage : BasePage
{
    public OrderHistoryPage()
    {
        InitializeComponent();
        BindingContext = new OrderHistoryViewModel();
    }
}