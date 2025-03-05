using MauiKit.ViewModels.Onboardings;

namespace MauiKit.Views.Onboardings;

public partial class WalkthroughStyle1Page : ContentPage
{
	public WalkthroughStyle1Page()
	{
		InitializeComponent();
		BindingContext = new WalkthroughStyle1ViewModel(Navigation, this);
    }

    protected override async void OnAppearing()
	{
        base.OnAppearing();
    }
}