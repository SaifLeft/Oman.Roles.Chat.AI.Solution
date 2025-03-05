namespace MauiKit.Views.Ecommerce;

public partial class CategoryDetailPage : BasePage
{
    public CategoryDetailPage()
    {
        InitializeComponent();
        BindingContext = new CategoryDetailViewModel();
    }
}