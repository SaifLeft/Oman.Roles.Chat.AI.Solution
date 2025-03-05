namespace MauiKit.Views;
public partial class ThemeSettingsPopupPage : PopupPage
{
    public ThemeSettingsPopupPage()
    {
        InitializeComponent();
        BindingContext = new ThemeSettingsPopupViewModel();
    }

    async void OnCloseSetting_Tapped(object sender, TappedEventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}
