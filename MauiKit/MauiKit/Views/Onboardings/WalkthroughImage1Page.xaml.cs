using MauiKit.ViewModels.Onboardings;

namespace MauiKit.Views.Onboardings;

public partial class WalkthroughImage1Page : ContentPage
{
	public WalkthroughImage1Page()
	{
		InitializeComponent();
		BindingContext = new WalkthroughImage1ViewModel(Navigation, this);

    }
}