
namespace MauiKit.Views.Actions;

public partial class ActionConfirmationPopup : PopupPage
{
	public ActionConfirmationPopup()
	{
		InitializeComponent();
	}

    private async void AcceptButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }

    private async void RejectButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}