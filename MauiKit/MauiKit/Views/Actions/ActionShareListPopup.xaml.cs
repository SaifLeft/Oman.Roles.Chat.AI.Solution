
namespace MauiKit.Views.Actions;

public partial class ActionShareListPopup : PopupPage
{
	public ActionShareListPopup()
	{
		InitializeComponent();
	}

    private async void CloseButton_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}