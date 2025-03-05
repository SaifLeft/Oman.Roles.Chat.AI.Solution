
namespace MauiKit.Views.Actions;

public partial class ActionSurveysPopup : PopupPage
{
	public ActionSurveysPopup()
	{
		InitializeComponent();
	}

    private async void SubmitButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}