
namespace MauiKit.Views.Actions;

public partial class ActionSuccessPopup : PopupPage
{
	public ActionSuccessPopup()
	{
		InitializeComponent();
	}

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}