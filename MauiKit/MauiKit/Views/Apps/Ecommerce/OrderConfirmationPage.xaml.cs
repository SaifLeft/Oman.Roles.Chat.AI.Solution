
using MauiKit.ViewModels.Ecommerce;

namespace MauiKit.Views.Ecommerce;

public partial class OrderConfirmationPage : BasePage
{
    private readonly OrderConfirmationViewModel _viewModel;
    public OrderConfirmationPage()
	{
        InitializeComponent();

        _viewModel = new OrderConfirmationViewModel();
        BindingContext = _viewModel;
    }

    private void NumericUpDown_OnValueChanged(object sender, int e)
    {
        _viewModel.OnOrderQuantityChanged(e);
    }
}