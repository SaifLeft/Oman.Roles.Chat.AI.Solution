using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using PanCardView;
using Microsoft.Maui.LifecycleEvents;
using MauiKit.Handlers;
using FFImageLoading.Maui;
using RGPopup.Maui.Extensions;
using CommunityToolkit.Maui.Core;
using Mopups.Hosting;
using Microsoft.Extensions.Logging;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
using Windows.UI.Core;
#endif

namespace MauiKit
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterDemoAppServices()
                .RegisterViewModels()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseSkiaSharp()
                .UseFFImageLoading()
#if !WINDOWS
                .UseMauiMaps()
#endif
                .UseCardsView()
                .UseBarcodeReader()
                .UseMauiRGPopup(config =>
                {
                    config.BackPressHandler = null;
                    config.FixKeyboardOverlap = true;
                })
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Poppins-Regular.otf", "RegularFont");
                    fonts.AddFont("Poppins-Medium.otf", "MediumFont");
                    fonts.AddFont("Poppins-SemiBold.otf", "SemiBoldFont");
                    fonts.AddFont("Poppins-Bold.otf", "BoldFont");
                    fonts.AddFont("Caveat-Bold.ttf", "HandwritingBoldFont");
                    fonts.AddFont("Caveat-Regular.ttf", "HandwritingFont");

                    fonts.AddFont("fa-solid-900.ttf", "FaPro");
                    fonts.AddFont("fa-brands-400.ttf", "FaBrands");
                    fonts.AddFont("fa-regular-400.ttf", "FaRegular");
                    fonts.AddFont("line-awesome.ttf", "LineAwesome");
                    fonts.AddFont("material-icons-outlined-regular.otf", "MaterialDesign");
                    fonts.AddFont("ionicons.ttf", "IonIcons");
                    fonts.AddFont("icon.ttf", "MauiKitIcons");
                })
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddHandler(typeof(ProgressBar), typeof(ProgressBarHandler));
                    handlers.AddHandler(typeof(CameraBarcodeReaderView), typeof(CameraBarcodeReaderViewHandler));
                    handlers.AddHandler(typeof(CameraView), typeof(CameraViewHandler));
                    handlers.AddHandler(typeof(BarcodeGeneratorView), typeof(BarcodeGeneratorViewHandler));
                })
                .ConfigureLifecycleEvents(events =>
                {
#if ANDROID
                    events.AddAndroid(android => android.OnCreate((activity, bundle) => MakeStatusBarTranslucent(activity)));

                    static void MakeStatusBarTranslucent(Android.App.Activity activity)
                    {
                        //activity.Window.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits, Android.Views.WindowManagerFlags.LayoutNoLimits);

                        activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);

                        activity.Window.SetStatusBarColor(Android.Graphics.Color.White);

                        activity.Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
                    }
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            //#if IOS
            //Microsoft.Maui.Handlers.ScrollViewHandler.Mapper.AppendToMapping("ContentSize", (handler,view) =>
            //{
            //    handler.PlatformView.UpdateContentSize(handler.VirtualView.ContentSize);
            //    handler.PlatformArrange(handler.PlatformView.Frame.ToRectangle());
            //});
            //#endif

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

                        //https://github.com/microsoft/microsoft-ui-xaml/issues/8746
                        ///
                        /// System back button for backwards navigation is no longer recommend
                        /// Instead, you should provide your own in-app back button
                        //  https://learn.microsoft.com/en-us/windows/apps/design/basics/navigation-history-and-backwards-navigation?source=recommendations#system-back-behavior-for-backward-compatibility
                        var titleBar = winuiAppWindow.TitleBar;
                        titleBar.ExtendsContentIntoTitleBar = true;
                        titleBar.BackgroundColor = Windows.UI.Color.FromArgb(1, 186, 213, 248); //Hex: #BAD5F8
                        titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(1, 186, 213, 248); //Hex: #BAD5F8
                        titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Black;
                        titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(1, 186, 213, 248); //Hex: #BAD5F8
                        titleBar.ForegroundColor = Microsoft.UI.Colors.White;
                        titleBar.InactiveBackgroundColor = Microsoft.UI.Colors.Black;

                        //https://github.com/dotnet/maui/issues/6976
                        var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(win32WindowsId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                        
                        int width = displayArea.WorkArea.Width * 3 / 4;
                        int height = displayArea.WorkArea.Height - 50;

                        winuiAppWindow.MoveAndResize(new RectInt32(25, 50, width, height));

                    });
                });
#endif
            });

            builder.Services.AddLocalization();

            // Registration in MauiProgram.cs
            //builder.Services.AddTransientPopup<ThemeSettingsPopupPage, ThemeSettingsPopupViewModel>();
            //builder.Services.AddTransientPopup<LanguageSelectionPopupPage, LanguageSelectionPopupViewModel>();

            var app = builder.Build();

            return app;
        }

        public static MauiAppBuilder RegisterDemoAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            //mauiAppBuilder.Services.AddSingleton<AppShell>();
            mauiAppBuilder.Services.AddSingleton<AppFlyout>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<MainMenuPage>();
            mauiAppBuilder.Services.AddTransient<MainPage>();
            mauiAppBuilder.Services.AddTransient<AppIndexPage>();

            mauiAppBuilder.Services.AddTransient<TestPage>();
            mauiAppBuilder.Services.AddTransient<TestPageViewModel>();

            mauiAppBuilder.Services.AddTransient<ArticleParallaxHeaderPage>();
            mauiAppBuilder.Services.AddTransient<ArticleDetailCardPage>();
            mauiAppBuilder.Services.AddTransient<ArticleDetailVideoPage>();
            mauiAppBuilder.Services.AddTransient<ArticleCurvedMaskPage>();
            mauiAppBuilder.Services.AddTransient<ArticlesClassicPage>();
            mauiAppBuilder.Services.AddTransient<ArticlesCardOverlayPage>();
            mauiAppBuilder.Services.AddTransient<ArticlesVideoPage>();
            mauiAppBuilder.Services.AddTransient<VideoPlayerPage>();

            mauiAppBuilder.Services.AddTransient<SocialProfilePage>();
            mauiAppBuilder.Services.AddTransient<SocialProfileGalleryPage>();
            mauiAppBuilder.Services.AddTransient<ChatHomePage>();
            mauiAppBuilder.Services.AddTransient<ChatDetailPage>();
            mauiAppBuilder.Services.AddTransient<ContactDetailPage>();

            mauiAppBuilder.Services.AddTransient<EcommerceIndexPage>();
            mauiAppBuilder.Services.AddTransient<EcommerceHomePage>();
            mauiAppBuilder.Services.AddTransient<ProductDetailPage>();
            mauiAppBuilder.Services.AddTransient<OrderHistoryPage>();
            mauiAppBuilder.Services.AddTransient<EcommerceProfilePage>();

            mauiAppBuilder.Services.AddTransient<TravelsIndexPage>();
            mauiAppBuilder.Services.AddTransient<TravelHomePage>();
            mauiAppBuilder.Services.AddTransient<TravelExplorePage>();
            mauiAppBuilder.Services.AddTransient<TravelAccountPage>();
            mauiAppBuilder.Services.AddTransient<TravelSettingsPage>();
            mauiAppBuilder.Services.AddTransient<TravelMessagesPage>();
            mauiAppBuilder.Services.AddTransient<TravelMessageDetailPage>();
            mauiAppBuilder.Services.AddTransient<TravelArticlePage>();

            mauiAppBuilder.Services.AddTransient<TravelHomeViewModel>();
            mauiAppBuilder.Services.AddTransient<TravelExploreViewModel>();
            mauiAppBuilder.Services.AddTransient<TravelArticleViewModel>();
            mauiAppBuilder.Services.AddTransient<TravelAccountViewModel>();
            mauiAppBuilder.Services.AddTransient<TravelSettingsViewModel>();
            mauiAppBuilder.Services.AddTransient<TravelMessagesViewModel>();
            mauiAppBuilder.Services.AddTransient<TravelMessageDetailViewModel>();

            mauiAppBuilder.Services.AddTransient<DashboardTimelinePage>();
            mauiAppBuilder.Services.AddTransient<DashboardVisualPage>();
            mauiAppBuilder.Services.AddTransient<DashboardGridPage>();
            mauiAppBuilder.Services.AddTransient<DashboardCarouselPage>();
            mauiAppBuilder.Services.AddTransient<DashboardVariantsPage>();
            mauiAppBuilder.Services.AddTransient<DashboardTimelineViewModel>();
            mauiAppBuilder.Services.AddTransient<DashboardVisualViewModel>();
            mauiAppBuilder.Services.AddTransient<DashboardGridViewModel>();
            mauiAppBuilder.Services.AddTransient<DashboardCarouselViewModel>();
            mauiAppBuilder.Services.AddTransient<DashboardVariantsViewModel>();

            mauiAppBuilder.Services.AddTransient<MainMenuViewModel>();
            mauiAppBuilder.Services.AddTransient<MainViewModel>();


            mauiAppBuilder.Services.AddTransient<ArticleParallaxHeaderViewModel>();
            mauiAppBuilder.Services.AddTransient<ArticleDetailVideoViewModel>();
            mauiAppBuilder.Services.AddTransient<ArticleDetailCardViewModel>();
            mauiAppBuilder.Services.AddTransient<ArticleCurvedMaskViewModel>();
            mauiAppBuilder.Services.AddTransient<ArticlesClassicViewModel>();
            mauiAppBuilder.Services.AddTransient<ArticlesCardOverlayViewModel>();
            mauiAppBuilder.Services.AddTransient<ArticlesVideoViewModel>();
            mauiAppBuilder.Services.AddTransient<VideoPlayerViewModel>();

            mauiAppBuilder.Services.AddTransient<ChatHomeViewModel>();
            mauiAppBuilder.Services.AddTransient<ChatDetailViewModel>();
            mauiAppBuilder.Services.AddTransient<SocialProfileGalleryViewModel>();
            mauiAppBuilder.Services.AddTransient<SocialProfileViewModel>();
            mauiAppBuilder.Services.AddTransient<ContactDetailViewModel>();

            mauiAppBuilder.Services.AddTransient<NewsHomeViewModel>();
            mauiAppBuilder.Services.AddTransient<CategoriesViewModel>();
            mauiAppBuilder.Services.AddTransient<VideoNewsViewModel>();
            mauiAppBuilder.Services.AddTransient<NewsDetailViewModel>();
            mauiAppBuilder.Services.AddTransient<BookmarksViewModel>();
            mauiAppBuilder.Services.AddTransient<AuthorsViewModel>();
            mauiAppBuilder.Services.AddTransient<NewsProfileViewModel>();

            mauiAppBuilder.Services.AddTransient<EcommerceHomeViewModel>();
            mauiAppBuilder.Services.AddTransient<OrderConfirmationViewModel>();
            mauiAppBuilder.Services.AddTransient<CheckoutViewModel>();
            mauiAppBuilder.Services.AddTransient<ProductDetailViewModel>();
            mauiAppBuilder.Services.AddTransient<OrderHistoryViewModel>();
            mauiAppBuilder.Services.AddTransient<EcommerceProfileViewModel>();

            mauiAppBuilder.Services.AddTransient<PropertyHomePage>();
            mauiAppBuilder.Services.AddTransient<PropertyDetailPage>();
            mauiAppBuilder.Services.AddTransient<PropertyBookingPage>();
            mauiAppBuilder.Services.AddTransient<AgentProfilePage>();
            mauiAppBuilder.Services.AddTransient<PropertyHomeViewModel>();
            mauiAppBuilder.Services.AddTransient<PropertyDetailViewModel>();
            mauiAppBuilder.Services.AddTransient<PropertyBookingViewModel>();
            mauiAppBuilder.Services.AddTransient<AgentProfileViewModel>();

            mauiAppBuilder.Services.AddTransient<DemoWalkthroughViewModel>();
            mauiAppBuilder.Services.AddTransient<WalkthroughAnimationViewModel>();
            mauiAppBuilder.Services.AddTransient<WalkthroughIllustrationViewModel>();
            mauiAppBuilder.Services.AddTransient<WalkthroughGradientViewModel>();
            mauiAppBuilder.Services.AddTransient<WalkthroughImage1ViewModel>();
            mauiAppBuilder.Services.AddTransient<WalkthroughImage2ViewModel>();

            mauiAppBuilder.Services.AddTransient<NewsHomePage>();
            mauiAppBuilder.Services.AddTransient<CategoriesPage>();
            mauiAppBuilder.Services.AddTransient<VideoNewsPage>();
            mauiAppBuilder.Services.AddTransient<NewsDetailPage>();
            mauiAppBuilder.Services.AddTransient<BookmarksPage>();
            mauiAppBuilder.Services.AddTransient<AuthorsPage>();
            mauiAppBuilder.Services.AddTransient<ProfilePage>();

            mauiAppBuilder.Services.AddTransient<DemoStartPage>();
            mauiAppBuilder.Services.AddTransient<DemoWalkthroughPage>();
            mauiAppBuilder.Services.AddTransient<WalkthroughAnimationPage>();
            mauiAppBuilder.Services.AddTransient<WalkthroughGradientPage>();
            mauiAppBuilder.Services.AddTransient<WalkthroughImage1Page>();
            mauiAppBuilder.Services.AddTransient<WalkthroughIllustrationPage>();
            mauiAppBuilder.Services.AddTransient<WalkthroughImage2Page>();
            mauiAppBuilder.Services.AddTransient<StartPage>();
            mauiAppBuilder.Services.AddTransient<StartBackgroundPage>();

            return mauiAppBuilder;
        }
    }
}