
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;

namespace MauiKit.Views.Onboardings;

public partial class WalkthroughPage : ContentPage
{
    public WalkthroughPage()
	{
		InitializeComponent();
		BindingContext = new WalkthroughViewModel(Navigation, this);
    }
}