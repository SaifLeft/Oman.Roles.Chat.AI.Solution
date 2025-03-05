namespace MauiKit.Views.Ecommerce;

public partial class EcommerceProfilePage : BasePage
{
    public EcommerceProfilePage()
    {
        InitializeComponent();
        BindingContext = new EcommerceProfileViewModel();
    }

    private async void Logout_Tapped(object sender, EventArgs e)
    {
        //Todo: logout logic
    }
}