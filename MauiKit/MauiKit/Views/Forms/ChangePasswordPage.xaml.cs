namespace MauiKit.Views.Forms;

public partial class ChangePasswordPage : BasePage
{
	public ChangePasswordPage()
	{
		InitializeComponent();
	}

    private void ButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
}