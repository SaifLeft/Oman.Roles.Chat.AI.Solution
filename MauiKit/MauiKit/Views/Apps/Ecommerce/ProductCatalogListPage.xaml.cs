namespace MauiKit.Views.Ecommerce;

public partial class ProductCatalogListPage : BasePage
{
    public ProductCatalogListPage()
    {
        InitializeComponent();
        BindingContext = new ProductCatalogViewModel();
    }
}