
namespace MauiKit.Views.Actions;

public partial class ActionWarningBackgroundPopup : PopupPage
{
	public ActionWarningBackgroundPopup()
	{
		InitializeComponent();
	}

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}