namespace MauiKit.Views.Properties;

public partial class PropertySettingsPage : BasePage
{
    public PropertySettingsPage()
    {
        InitializeComponent();
        BindingContext = new PropertySettingsViewModel();
    }

    private async void BackButton_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void Logout_Tapped(object sender, EventArgs e)
    {
        //Todo: logout logic
    }
}