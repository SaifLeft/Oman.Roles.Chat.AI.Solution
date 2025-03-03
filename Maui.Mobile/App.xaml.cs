using Maui.Service;

namespace Maui.Mobile
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;

        public App(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;

            MainPage = new AppShell();

            // Set the initial page based on authentication status
            CheckAuthStatusAndSetInitialPage();
        }

        private async void CheckAuthStatusAndSetInitialPage()
        {
            // Check if the user is already authenticated
            bool isAuthenticated = await _authService.IsAuthenticatedAsync();

            if (isAuthenticated)
            {
                // User is already logged in, navigate to MainPage
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                // User is not logged in, navigate to LoginPage
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}