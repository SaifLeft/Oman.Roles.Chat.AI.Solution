
namespace MauiKit.Views.Actions;

public partial class ActionCallPopup : PopupPage
{
	public ActionCallPopup()
	{
		InitializeComponent();
	}

    private async void CallButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}