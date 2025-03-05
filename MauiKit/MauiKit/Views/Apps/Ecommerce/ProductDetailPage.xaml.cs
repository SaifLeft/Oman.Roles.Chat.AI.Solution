using MauiKit.ViewModels.Ecommerce;

namespace MauiKit.Views.Ecommerce;

public partial class ProductDetailPage : BasePage
{
    ProductDetailViewModel viewModel;
    public ProductDetailPage()
    {
        InitializeComponent();
        BindingContext = viewModel = new ProductDetailViewModel();
    }

    private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        viewModel.ChageFooterVisibility(e.ScrollY);
    }

    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}