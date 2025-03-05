using MauiKit.ViewModels.Ecommerce;

namespace MauiKit.Views.Ecommerce;

public partial class EcommerceHomePage : BasePage
{
    public EcommerceHomePage()
    {
        InitializeComponent();
        BindingContext = new EcommerceHomeViewModel();
    }
}