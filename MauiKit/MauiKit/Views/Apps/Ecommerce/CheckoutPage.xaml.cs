using MauiKit.ViewModels.Ecommerce;

namespace MauiKit.Views.Ecommerce;

public partial class CheckoutPage : BasePage
{
    private readonly CheckoutViewModel _viewModel;
    public CheckoutPage()
	{
		InitializeComponent();
        _viewModel = new CheckoutViewModel();
        BindingContext = _viewModel;
    }

    private void NumericUpDown_OnValueChanged(object sender, int e)
    {
        _viewModel.OnOrderQuantityChanged(e);
    }

    private async void ShippingInfo_Tapped(object sender, EventArgs e)
    {

    }

    private async void ShippingInfoSubmit_Clicked(object sender, EventArgs e)
    {
        //Todo: add submit function

    }

    private async void PaymentMethod_Tapped(object sender, EventArgs e)
    {

    }

    private async void PaymentMethodOk_Clicked(object sender, EventArgs e)
    {
        //Todo: add submit function
    }
}