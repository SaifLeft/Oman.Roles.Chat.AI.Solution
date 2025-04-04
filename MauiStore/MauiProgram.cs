using Microsoft.Extensions.Logging;
using MauiStore.Shared.Services;
using MauiStore.Services;
using MudBlazor.Services;
using Blazored.LocalStorage;
using MauiStore.Infrastructure;
using Microsoft.Maui.LifecycleEvents;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace MauiStore;

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
            });

        // Add device-specific services used by the MauiStore.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<ClientPreferenceManager>();

        builder.ConfigureLifecycleEvents(events =>
        {
#if WINDOWS
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                        AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);

                        //https://github.com/dotnet/maui/issues/7751
                        window.ExtendsContentIntoTitleBar = false; // must be false or else you see some of the buttons
                        winuiAppWindow.SetPresenter(AppWindowPresenterKind.Default);

                        //https://github.com/dotnet/maui/issues/6976
                        var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(win32WindowsId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                        
                        int width = displayArea.WorkArea.Width * 3 / 4;
                        int height = displayArea.WorkArea.Height - 10;

                        winuiAppWindow.MoveAndResize(new RectInt32(15, 10, width, height));
                    });
                });
#endif
#if ANDROID
                   events.AddAndroid(android => android.OnCreate((activity, bundle) => MakeStatusBarTranslucent(activity)));

                    static void MakeStatusBarTranslucent(Android.App.Activity activity)
                    {
                        //activity.Window.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits, Android.Views.WindowManagerFlags.LayoutNoLimits);
                        activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                        activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentNavigation);
                        //activity.Window.SetStatusBarColor(Android.Graphics.Color.White);
                        activity.Window.SetNavigationBarColor(Android.Graphics.Color.Black);
                    } 
#endif
        });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
