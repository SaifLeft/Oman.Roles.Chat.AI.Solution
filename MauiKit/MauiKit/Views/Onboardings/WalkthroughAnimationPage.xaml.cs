using MauiKit.ViewModels.Onboardings;

namespace MauiKit.Views.Onboardings;

public partial class WalkthroughAnimationPage : ContentPage
{
	public WalkthroughAnimationPage()
	{
		InitializeComponent();
		BindingContext = new WalkthroughAnimationViewModel(Navigation, this);
		
    }
}