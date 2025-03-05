
namespace MauiKit.Views;

public partial class RemarkPage : ContentPage
{
    public RemarkPage()
	{
		InitializeComponent();
        BindingContext = new RemarkViewModel();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    
}

