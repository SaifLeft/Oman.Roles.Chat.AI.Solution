
namespace MauiKit.Views.Actions;

public partial class ActionShareGridPopup : PopupPage
{
	public ActionShareGridPopup()
	{
		InitializeComponent();
	}

    private async void CloseButton_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}