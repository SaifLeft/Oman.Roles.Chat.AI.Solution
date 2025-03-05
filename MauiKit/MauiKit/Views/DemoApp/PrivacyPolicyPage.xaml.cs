namespace MauiKit.Views;
public partial class PrivacyPolicyPage : ContentPage
{
	
	public PrivacyPolicyPage()
	{
		InitializeComponent();
        BindingContext = new PrivacyPolicyViewModel();
    }
}

