namespace MauiKit.Views.Actions;

public partial class ActionWelcomePopupPage : PopupPage
{
	public ActionWelcomePopupPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}