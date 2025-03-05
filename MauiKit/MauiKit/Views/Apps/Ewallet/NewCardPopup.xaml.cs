namespace MauiKit.Views;
public partial class NewCardPopup : PopupPage
{
	public NewCardPopup()
	{
		InitializeComponent();
	}

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}