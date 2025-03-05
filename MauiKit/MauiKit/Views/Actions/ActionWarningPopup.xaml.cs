
namespace MauiKit.Views.Actions;

public partial class ActionWarningPopup : PopupPage
{
	public ActionWarningPopup()
	{
		InitializeComponent();
	}

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}