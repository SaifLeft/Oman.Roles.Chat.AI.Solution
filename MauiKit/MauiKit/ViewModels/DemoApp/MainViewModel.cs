namespace MauiKit.ViewModels;
public partial class MainViewModel : BaseViewModel, IRecipient<CultureChangeMessage>
{
    [ObservableProperty]
    private List<HomeBanner> _bannerItems;
    
    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    string text;

    [ObservableProperty]
    string uri;

    [ObservableProperty]
    string subject;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    bool isRTLLanguage;

    public ICommand ShareCommand { get; }
    public ICommand BuyNowCommand { get; }
    public MainViewModel()
    {
        LoadBannerCollection();
        WeakReferenceMessenger.Default.Register<CultureChangeMessage>(this);
        IsRTLLanguage = AppSettings.IsRTLLanguage;
        
        ShareCommand = new Command<Microsoft.Maui.Controls.View>(OnShare);
        BuyNowCommand = new Command<Microsoft.Maui.Controls.View>(OnBuyNow);
    }

    /// <summary>
    /// On received culture changed message, reload banner items
    /// </summary>
    /// <param name="message"></param>
    public void Receive(CultureChangeMessage message)
    {
        IsRTLLanguage = AppSettings.IsRTLLanguage;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LoadBannerCollection();
        });
    }

    public void LoadBannerCollection()
    {
        BannerItems = new List<HomeBanner> 
        { 
            new HomeBanner
            {
                Title = LocalizationResourceManager.Translate("StringBannerEcommerce"),
                Icon = IonIcons.Bag,
                BackgroundGradient = new LinearGradientBrush
                {
                    // StartPoint defaults to (0,0). EndPoint defaults to (1,1).
                    GradientStops =
                    {
                        new GradientStop { Color = Color.FromArgb("#843cf7"), Offset = 0.1f },
                        new GradientStop { Color = Color.FromArgb("#38b8f2"), Offset = 1.0f }
                    }
                },
                BackgroundGradientStart = Color.FromArgb("#38b8f2"),
                BackgroundGradientEnd = Color.FromArgb("#843cf7"),
                Body = LocalizationResourceManager.Translate("StringBannerEcommerceDescription"),
                BackgroundImage = AppSettings.ImageServerPath + "background/bg_01.png"
            },
            new HomeBanner
            {
                Title = LocalizationResourceManager.Translate("StringBannerNews"),
                Icon = IonIcons.SocialDesignernewsOutline,
                BackgroundGradient = new LinearGradientBrush
                {
                    // StartPoint defaults to (0,0). EndPoint defaults to (1,1).
                    GradientStops =
                    {
                        new GradientStop { Color = Color.FromArgb("#ff6a00"), Offset = 0.1f },
                        new GradientStop { Color = Color.FromArgb("#ee0979"), Offset = 1.0f }
                    }
                },
                BackgroundGradientStart = Color.FromArgb("#ee0979"),
                BackgroundGradientEnd = Color.FromArgb("#ff6a00"),
                Body = LocalizationResourceManager.Translate("StringBannerNewsDescription"),
                BackgroundImage = AppSettings.ImageServerPath + "background/bg_02.png"
            },
            new HomeBanner
            {
                Title = LocalizationResourceManager.Translate("StringBannerPropertyListing"),
                Icon = IonIcons.IosHomeOutline,
                BackgroundGradient = new LinearGradientBrush
                {
                    // StartPoint defaults to (0,0). EndPoint defaults to (1,1).
                    GradientStops =
                    {
                        new GradientStop { Color = Color.FromArgb("#78cc37"), Offset = 0.1f },
                        new GradientStop { Color = Color.FromArgb("#004b91"), Offset = 1.0f }
                    }
                },
                BackgroundGradientStart = Color.FromArgb("#004b91"),
                BackgroundGradientEnd = Color.FromArgb("#78cc37"),
                Body = LocalizationResourceManager.Translate("StringBannerPropertyListingDescription"),
                BackgroundImage = AppSettings.ImageServerPath + "background/bg_03.png"
            },
            new HomeBanner
            {
                Title = LocalizationResourceManager.Translate("StringBannerSocials"),
                Icon = IonIcons.Chatboxes,
                BackgroundGradient = new LinearGradientBrush
                {
                    // StartPoint defaults to (0,0). EndPoint defaults to (1,1).
                    GradientStops =
                    {
                        new GradientStop { Color = Color.FromArgb("#FF396f"), Offset = 0.1f },
                        new GradientStop { Color = Color.FromArgb("#9B3cb7"), Offset = 1.0f }
                    }
                },
                BackgroundGradientStart = Color.FromArgb("#9B3cb7"),
                BackgroundGradientEnd = Color.FromArgb("#FF396f"),
                Body = LocalizationResourceManager.Translate("StringBannerSocialsDescription"),
                BackgroundImage = AppSettings.ImageServerPath + "background/bg_04.png"
            },
            new HomeBanner
            {
                Title = LocalizationResourceManager.Translate("StringBannerArticles"),
                Icon = IonIcons.Filing,
                BackgroundGradient = new LinearGradientBrush
                {
                    // StartPoint defaults to (0,0). EndPoint defaults to (1,1).
                    GradientStops =
                    {
                        new GradientStop { Color = Color.FromArgb("#59D3FC"), Offset = 0.1f },
                        new GradientStop { Color = Color.FromArgb("#554DDE"), Offset = 1.0f }
                    }
                },
                BackgroundGradientStart = Color.FromArgb("#554DDE"),
                BackgroundGradientEnd = Color.FromArgb("#59D3FC"),
                Body = LocalizationResourceManager.Translate("StringBannerArticlesDescription"),
                BackgroundImage = AppSettings.ImageServerPath + "background/bg_05.png"
            }
        };
    }


    async void OnShare(Microsoft.Maui.Controls.View element)
        => await Share.RequestAsync(new ShareTextRequest
        {
            Subject = "Share MauiKit app",
            Text = "Hi, checkout this awesome UI kit!",
            Uri = "https://play.google.com/store/apps/details?id=com.tls.mauikit",
            Title = "Share MauiKit app",
            PresentationSourceBounds = element.GetAbsoluteBounds()
        });
  
    async void OnBuyNow(Microsoft.Maui.Controls.View element)
    {
        var url = "https://1.envato.market/Y9qgBO";
        await Launcher.OpenAsync(url);
    }
}
