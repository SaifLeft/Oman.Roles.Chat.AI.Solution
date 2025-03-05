namespace MauiKit.Views.Ecommerce;

public partial class BrandDetail : BasePage
{
    public BrandDetail()
    {
        InitializeComponent();
        BindingContext = new BrandDetailViewModel();
    }
}