namespace MauiKit.Views.Actions;

public partial class ActionCongratulationPopupPage : PopupPage
{
	public ActionCongratulationPopupPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}