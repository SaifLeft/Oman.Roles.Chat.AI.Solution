
using MauiKit.Views.App;

namespace MauiKit.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void OnSettingsToolbarItemClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ThemeSettingsPage());
    }

    async void AboutUs_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AboutPage());
    }

    private async Task Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChatWithAI());
    }
}

