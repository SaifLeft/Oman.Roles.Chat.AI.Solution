

using MauiKit.Views.Samples;

namespace MauiKit;

public partial class AppShell : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public ICommand ShareCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
    public AppShell()
	{
		InitializeComponent();
        RegisterRoutes();
        BindingContext = this;
    }

    void RegisterRoutes()
    {
        Routes.Add(nameof(AppIndexPage), typeof(AppIndexPage));
        //App Ecommerce
        Routes.Add(nameof(EcommerceIndexPage), typeof(EcommerceIndexPage));
        Routes.Add(nameof(EcommerceHomePage), typeof(EcommerceHomePage));
        Routes.Add(nameof(EcommerceProfilePage), typeof(EcommerceProfilePage));
        Routes.Add(nameof(ProductCatalogGridPage), typeof(ProductCatalogGridPage));
        Routes.Add(nameof(ProductCatalogListPage), typeof(ProductCatalogListPage));
        Routes.Add(nameof(ProductDetailPage), typeof(ProductDetailPage));
        Routes.Add(nameof(CategoryDetailPage), typeof(CategoryDetailPage));
        Routes.Add(nameof(OrderConfirmationPage), typeof(OrderConfirmationPage));
        Routes.Add(nameof(OrderHistoryPage), typeof(OrderHistoryPage));
        Routes.Add(nameof(TrackOrderPage), typeof(TrackOrderPage));
        Routes.Add(nameof(CartPage), typeof(CartPage));
        Routes.Add(nameof(CheckoutPage), typeof(CheckoutPage));
        Routes.Add(nameof(BrandDetail), typeof(BrandDetail));

        //App Ewallet
        Routes.Add(nameof(EwalletIndexPage), typeof(EwalletIndexPage));
        Routes.Add(nameof(EwalletHomePage), typeof(EwalletHomePage));
        Routes.Add(nameof(EwalletServicesPage), typeof(EwalletServicesPage));
        Routes.Add(nameof(EReceiptPage), typeof(EReceiptPage));
        Routes.Add(nameof(BillPaymentPage), typeof(BillPaymentPage));
        Routes.Add(nameof(MobileTopupPage), typeof(MobileTopupPage));
        Routes.Add(nameof(MyCardsPage), typeof(MyCardsPage));
        Routes.Add(nameof(PaymentConfirmPage), typeof(PaymentConfirmPage));
        Routes.Add(nameof(ScanQrPayPage), typeof(ScanQrPayPage));
        Routes.Add(nameof(StatisticsPage), typeof(StatisticsPage));
        Routes.Add(nameof(TransferMoneyPage), typeof(TransferMoneyPage));

        //App News
        Routes.Add(nameof(NewsIndexPage), typeof(NewsIndexPage));
        Routes.Add(nameof(NewsHomePage), typeof(NewsHomePage));
        Routes.Add(nameof(NewsDetailPage), typeof(NewsDetailPage));
        Routes.Add(nameof(AuthorsPage), typeof(AuthorsPage));
        Routes.Add(nameof(BookmarksPage), typeof(BookmarksPage));
        Routes.Add(nameof(CategoriesPage), typeof(CategoriesPage));
        Routes.Add(nameof(NewsProfilePage), typeof(NewsProfilePage));
        Routes.Add(nameof(VideoNewsPage), typeof(VideoNewsPage));

        //App Properties
        Routes.Add(nameof(PropertyIndexPage), typeof(PropertyIndexPage));
        Routes.Add(nameof(AgentProfilePage), typeof(AgentProfilePage));
        Routes.Add(nameof(BookingPaymentPage), typeof(BookingPaymentPage));
        Routes.Add(nameof(PropertyBookingPage), typeof(PropertyBookingPage));
        Routes.Add(nameof(PropertyDetailPage), typeof(PropertyDetailPage));
        Routes.Add(nameof(PropertyHomePage), typeof(PropertyHomePage));
        Routes.Add(nameof(PropertyListingCardPage), typeof(PropertyListingCardPage));
        Routes.Add(nameof(PropertyListingClasicPage), typeof(PropertyListingClasicPage));
        Routes.Add(nameof(PropertyMapPage), typeof(PropertyMapPage));
        Routes.Add(nameof(PropertySettingsPage), typeof(PropertySettingsPage));

        //App Travel
        Routes.Add(nameof(TravelsIndexPage), typeof(TravelsIndexPage));
        Routes.Add(nameof(TravelAccountPage), typeof(TravelAccountPage));
        Routes.Add(nameof(TravelArticlePage), typeof(TravelArticlePage));
        Routes.Add(nameof(TravelExplorePage), typeof(TravelExplorePage));
        Routes.Add(nameof(TravelFavoritesPage), typeof(TravelFavoritesPage));
        Routes.Add(nameof(TravelHomePage), typeof(TravelHomePage));
        Routes.Add(nameof(TravelMessageDetailPage), typeof(TravelMessageDetailPage));
        Routes.Add(nameof(TravelMessagesPage), typeof(TravelMessagesPage));
        Routes.Add(nameof(TravelSettingsPage), typeof(TravelSettingsPage));

        //Actions
        Routes.Add(nameof(ActionsIndexPage), typeof(ActionsIndexPage));

        //Articles
        Routes.Add(nameof(ArticlesPage), typeof(ArticlesPage));
        Routes.Add(nameof(AddArticlePage), typeof(AddArticlePage));
        Routes.Add(nameof(ArticleCurvedMaskPage), typeof(ArticleCurvedMaskPage));
        Routes.Add(nameof(ArticleDetailCardPage), typeof(ArticleDetailCardPage));
        Routes.Add(nameof(ArticleDetailVideoPage), typeof(ArticleDetailVideoPage));
        Routes.Add(nameof(ArticleParallaxHeaderPage), typeof(ArticleParallaxHeaderPage));
        Routes.Add(nameof(ArticlesCardOverlayPage), typeof(ArticlesCardOverlayPage));
        Routes.Add(nameof(ArticlesClassicPage), typeof(ArticlesClassicPage));
        Routes.Add(nameof(ArticlesVideoPage), typeof(ArticlesVideoPage));
        Routes.Add(nameof(VideoDetailPage), typeof(VideoDetailPage));
        Routes.Add(nameof(VideoPlayerPage), typeof(VideoPlayerPage));

        //Charts
        Routes.Add(nameof(ChartsPage), typeof(ChartsPage));
        Routes.Add(nameof(CartesianChartsPage), typeof(CartesianChartsPage));
        Routes.Add(nameof(GeoMapPage), typeof(GeoMapPage));
        Routes.Add(nameof(PieChartsPage), typeof(PieChartsPage));
        Routes.Add(nameof(PolarChartsPage), typeof(PolarChartsPage));

        //Dashboards
        Routes.Add(nameof(DashboardsPage), typeof(DashboardsPage));
        Routes.Add(nameof(AndroidBottomTabbedPage), typeof(AndroidBottomTabbedPage));
        Routes.Add(nameof(DashboardArticlePage), typeof(DashboardArticlePage));
        Routes.Add(nameof(DashboardCardPage), typeof(DashboardCardPage));
        Routes.Add(nameof(DashboardCarouselPage), typeof(DashboardCarouselPage));
        Routes.Add(nameof(DashboardCoverFlowPage), typeof(DashboardCoverFlowPage));
        Routes.Add(nameof(DashboardEventPage), typeof(DashboardEventPage));
        Routes.Add(nameof(DashboardGridPage), typeof(DashboardGridPage));
        Routes.Add(nameof(DashboardGridInversePage), typeof(DashboardGridInversePage));
        Routes.Add(nameof(DashboardTimelinePage), typeof(DashboardTimelinePage));
        Routes.Add(nameof(DashboardVariantsPage), typeof(DashboardVariantsPage));
        Routes.Add(nameof(DashboardVisualPage), typeof(DashboardVisualPage));
        Routes.Add(nameof(DefaultTabbedPage), typeof(DefaultTabbedPage));

        //FontIcons Page
        Routes.Add(nameof(FontIconsPage), typeof(FontIconsPage));

        //Forms
        Routes.Add(nameof(FormsPage), typeof(FormsPage));
        Routes.Add(nameof(BackgroundGradientLoginPage), typeof(BackgroundGradientLoginPage));
        Routes.Add(nameof(BackgroundGradientSignUpPage), typeof(BackgroundGradientSignUpPage));
        Routes.Add(nameof(FullBackgroundLoginPage), typeof(FullBackgroundLoginPage));
        Routes.Add(nameof(FullBackgroundSignUpPage), typeof(FullBackgroundSignUpPage));
        Routes.Add(nameof(LoginPage), typeof(LoginPage));
        Routes.Add(nameof(SignUpPage), typeof(SignUpPage));
        Routes.Add(nameof(SimpleLoginPage), typeof(SimpleLoginPage));
        Routes.Add(nameof(SimpleSignUpPage), typeof(SimpleSignUpPage));
        Routes.Add(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
        Routes.Add(nameof(PasswordVerificationPage), typeof(PasswordVerificationPage));
        Routes.Add(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
        Routes.Add(nameof(CalendarPage), typeof(CalendarPage));
        Routes.Add(nameof(VideoBackgroundLoginPage), typeof(VideoBackgroundLoginPage));
        Routes.Add(nameof(VideoBackgroundSignUpPage), typeof(VideoBackgroundSignUpPage));

        //Lists
        Routes.Add(nameof(ListsPage), typeof(ListsPage));
        Routes.Add(nameof(ListCardsPage), typeof(ListCardsPage));
        Routes.Add(nameof(ListFlatPage), typeof(ListFlatPage));
        Routes.Add(nameof(ListImagePage), typeof(ListImagePage));
        Routes.Add(nameof(ListIconPage), typeof(ListIconPage));

        //Onboardings
        Routes.Add(nameof(OnboardingsPage), typeof(OnboardingsPage));
        Routes.Add(nameof(DemoStartPage), typeof(DemoStartPage));
        Routes.Add(nameof(DemoWalkthroughPage), typeof(DemoWalkthroughPage));
        Routes.Add(nameof(StartPage), typeof(StartPage));
        Routes.Add(nameof(StartBackgroundPage), typeof(StartBackgroundPage));
        Routes.Add(nameof(WalkthroughAnimationPage), typeof(WalkthroughAnimationPage));
        Routes.Add(nameof(WalkthroughGradientPage), typeof(WalkthroughGradientPage));
        Routes.Add(nameof(WalkthroughIllustrationPage), typeof(WalkthroughIllustrationPage));
        Routes.Add(nameof(WalkthroughImage1Page), typeof(WalkthroughImage1Page));
        Routes.Add(nameof(WalkthroughImage2Page), typeof(WalkthroughImage2Page));
        Routes.Add(nameof(WalkthroughStyle1Page), typeof(WalkthroughStyle1Page));
        Routes.Add(nameof(WalkthroughStyle2Page), typeof(WalkthroughStyle2Page));

        //Socials
        Routes.Add(nameof(SocialsPage), typeof(SocialsPage));
        Routes.Add(nameof(ChatDetailPage), typeof(ChatDetailPage));
        Routes.Add(nameof(ChatHomePage), typeof(ChatHomePage));
        Routes.Add(nameof(ContactDetailPage), typeof(ContactDetailPage));
        Routes.Add(nameof(SocialProfileBackgroundCoverPage), typeof(SocialProfileBackgroundCoverPage));
        Routes.Add(nameof(SocialProfileCardPage), typeof(SocialProfileCardPage));
        Routes.Add(nameof(SocialProfileGalleryPage), typeof(SocialProfileGalleryPage));
        Routes.Add(nameof(SocialProfilePage), typeof(SocialProfilePage));

        //Others
        Routes.Add(nameof(AboutPage), typeof(AboutPage));
        Routes.Add(nameof(HomePage), typeof(HomePage));
        Routes.Add(nameof(MainPage), typeof(MainPage));
        Routes.Add(nameof(MainTabbedPage), typeof(MainTabbedPage));
        Routes.Add(nameof(PrivacyPolicyPage), typeof(PrivacyPolicyPage));
        Routes.Add(nameof(ProfilePage), typeof(ProfilePage));
        Routes.Add(nameof(RemarkPage), typeof(RemarkPage));
        Routes.Add(nameof(StatisticPage), typeof(StatisticPage));
        Routes.Add(nameof(TestPage), typeof(TestPage));
        Routes.Add(nameof(SamplePageOne), typeof(SamplePageOne));
        Routes.Add(nameof(SamplePageTwo), typeof(SamplePageTwo));
        Routes.Add(nameof(SamplePageThree), typeof(SamplePageThree));

        foreach (var item in Routes)
        {
            Routing.RegisterRoute(item.Key, item.Value);
        }
    }

}
