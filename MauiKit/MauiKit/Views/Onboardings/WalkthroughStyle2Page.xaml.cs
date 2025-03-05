using MauiKit.ViewModels.Onboardings;

namespace MauiKit.Views.Onboardings;

public partial class WalkthroughStyle2Page : ContentPage
{
	public WalkthroughStyle2Page()
	{
		InitializeComponent();
		BindingContext = new WalkthroughStyle2ViewModel(Navigation, this);
    }

    protected override async void OnAppearing()
	{
        base.OnAppearing();
    }
}