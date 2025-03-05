namespace MauiKit.Views.Actions;

public partial class ActionPasswordResetPopupPage : PopupPage
{
	public ActionPasswordResetPopupPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}