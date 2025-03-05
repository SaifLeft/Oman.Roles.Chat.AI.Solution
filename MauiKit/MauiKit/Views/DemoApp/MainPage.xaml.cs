
namespace MauiKit.Views;

public partial class MainPage : ContentPage
{
    private readonly AppTheme currentTheme;

    public MainPage()
	{
        currentTheme = Application.Current.RequestedTheme;
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

    async void AppFlowTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AppIndexPage());
    }
    async void ControlsTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ControlsOverviewPage());
    }
    async void ActionsTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ActionsIndexPage());
    }
    async void ChartTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChartsPage());
    }
    async void DashboardTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DashboardsPage());
    }
    async void ListsTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ListsPage());
    }
    async void ArticleTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticlesPage());
    }
    async void OnboardingTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OnboardingsPage());
    }
    async void FormsTapGestureRecognizer_Tapped(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new FormsPage());
    }

    async void FontIcons_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FontIconsPage());
    }
    async void SocialTapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SocialsPage());
    }
    
    async void AboutUs_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AboutPage());
    }

    private bool _rotated = false;

    private async void fabBtn_Clicked(object sender, EventArgs e)
    {
        ((Button)sender).RotateTo(_rotated ? 0 : -90);

        //fabBtnsContainer.Margin = new Thickness(0, 0, _rotated ? 0 : -100, 50);

        fabBtnsContainer.Animate<Thickness>("fab_btns",
            value => // goes from 0 -> 1
            {   // 0.1
                // 0.01,... 0.05,... 0.6,... 0.9,... 1

                int factor = Convert.ToInt32(value * 10); // 1

                var rightMargin =
                    !_rotated
                    ? (factor * 10) - 100  //10 - 100 => -90,    0.2 => 20-100 => -80 ..... 100 - 100 => 0
                    : (factor * 10) * -1; // 10*-1 => -10		20 *-1 => -20, ....... 100 * -1 => -100

                return new Thickness(0, 0, rightMargin, 85);
            },
            newThickness => fabBtnsContainer.Margin = newThickness // The parameter come from the previous Func delegate parameter
            , length: 200
            , finished: (_, __) => _rotated = !_rotated);
    }
}

