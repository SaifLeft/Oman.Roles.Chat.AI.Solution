using MauiKit.ViewModels.Ecommerce;

namespace MauiKit.Views.Ecommerce;

public partial class CartPage : BasePage
{
    public CartPage()
    {
        InitializeComponent();
        BindingContext = new CartViewModel();
    }
    private void SwipeItem_Invoked(object sender, EventArgs e)
    {

    }

    private void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        double value = e.NewValue;
    }
}