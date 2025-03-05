namespace MauiKit.Views.Ecommerce;

public partial class ProductCatalogGridPage : BasePage
{
    public ProductCatalogGridPage()
    {
        InitializeComponent();
        BindingContext = new ProductCatalogViewModel();
    }
}