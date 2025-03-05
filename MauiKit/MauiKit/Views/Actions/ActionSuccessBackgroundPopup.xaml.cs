
namespace MauiKit.Views.Actions;

public partial class ActionSuccessBackgroundPopup : PopupPage
{
	public ActionSuccessBackgroundPopup()
	{
		InitializeComponent();
	}

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}