using MauiKit.ViewModels.Onboardings;

namespace MauiKit.Views.Onboardings;

public partial class WalkthroughGradientPage : ContentPage
{
    public WalkthroughGradientPage()
	{
		InitializeComponent();
		BindingContext = new WalkthroughGradientViewModel(Navigation, this);
    }

    protected override async void OnAppearing()
    {

    }
}