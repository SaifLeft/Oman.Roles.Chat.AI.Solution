
namespace MauiKit.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
	{
		InitializeComponent();
        BindingContext = new HomeViewModel();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}

