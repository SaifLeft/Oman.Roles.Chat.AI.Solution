namespace MauiKit.Views.Socials;

public partial class ContactDetailPage : BasePage
{
	public ContactDetailPage()
	{
		try
        {
            InitializeComponent();
        }
        catch { Exception e; }
		BindingContext = new ContactDetailViewModel();
	}

    private async void OnEdit(object sender, EventArgs e)
    {
        await DisplayAlert("Edit tapped", "Navigate to the edit contact page.", "OK");
    }

    private async void OnClose(object sender, System.EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}