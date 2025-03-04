using API.Client;
using Maui.Mobile.ViewModels;
using Maui.Mobile.Views;
using Maui.Service;
using Maui.ViewModels;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace Maui.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();

            // Register HttpClient and API client
            builder.Services.AddSingleton<HttpClient>(CreateHttpClient());
            builder.Services.AddSingleton<IAPIClient, APIClient>();

            // Register view models
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<PhoneLoginViewModel>();

            // Register pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<PhoneLoginPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();

            // Configure base address and headers
            httpClient.BaseAddress = new Uri("https://api.smartlawyer.com/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Configure timeout
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            return httpClient;
        }
    }
}