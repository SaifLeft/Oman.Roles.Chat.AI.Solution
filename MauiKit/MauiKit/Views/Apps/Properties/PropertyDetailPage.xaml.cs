namespace MauiKit.Views.Properties;

public partial class PropertyDetailPage : BasePage
{
	public PropertyDetailPage()
	{
		InitializeComponent();
        BindingContext = new PropertyDetailViewModel();
    }

    private async void OnClose_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}