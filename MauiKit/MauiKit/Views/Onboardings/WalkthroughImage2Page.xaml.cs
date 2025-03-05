using MauiKit.ViewModels.Onboardings;

namespace MauiKit.Views.Onboardings;

public partial class WalkthroughImage2Page : ContentPage
{
	public WalkthroughImage2Page()
	{
		InitializeComponent();
		BindingContext = new WalkthroughImage2ViewModel(Navigation, this);
    }

    protected override async void OnAppearing()
	{
        base.OnAppearing();
    }
}