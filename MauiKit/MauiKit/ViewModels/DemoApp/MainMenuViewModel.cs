
namespace MauiKit.ViewModels;
public class MainMenuViewModel : ObservableObject, IRecipient<CultureChangeMessage>
{
    private INavigation _navigation;

    private Action<Page> _openPageAsRoot;

    private List<MenuEntry> _mainMenuEntries;

    private bool _isGridView;

    private MenuEntry _selectedMainMenuEntry;
    public MainMenuViewModel(INavigation navigation, Action<Page> openPageAsRoot)
    {
        _navigation = navigation;
        _openPageAsRoot = openPageAsRoot;

        IsGridMenuSwitchToggled = AppSettings.IsMenuGridStyle;

        WeakReferenceMessenger.Default.Register<CultureChangeMessage>(this);
        WeakReferenceMessenger.Default.Register<MainMenuGridStyleMessage>(this, (recipient, message) => {
            IsGridMenuSwitchToggled = (bool)message.Value;
        });

        LoadMenuData();

        var firstEntry = MainMenuEntries[0];
        MainMenuSelectedItem = firstEntry;

        MenuItemSelectionCommand = new Command<MenuEntry>(SelectedMenuItem);
    }

    /// <summary>
    /// On received culture changed message, reload Menu item
    /// </summary>
    /// <param name="message"></param>
    public void Receive(CultureChangeMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LoadMenuData();
        });
    }

    private void LoadMenuData()
    {
        MainMenuEntries = new List<MenuEntry>
        {
            new MenuEntry
            {
                Title = LocalizationResourceManager.Translate("MenuExplore"),
                Icon = MaterialDesignIcons.Explore,
                TargetType = typeof(MainPage)
            },
            new MenuEntry
            {
                Title = LocalizationResourceManager.Translate("MenuMainDashboard"),
                Icon = MaterialDesignIcons.Dashboard,
                TargetType = typeof(MainTabbedPage)
            },
            new MenuEntry
            {
                Title = LocalizationResourceManager.Translate("MenuApp"),
                Icon = MaterialDesignIcons.Tablet,
                TargetType = typeof(AppIndexPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuOnboarding"),
                Icon = MaterialDesignIcons.ViewCarousel,
                TargetType = typeof(OnboardingsPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuForm"),
                Icon = MaterialDesignIcons.NoteAdd,
                TargetType = typeof(FormsPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuAction"),
                Icon = MaterialDesignIcons.TouchApp,
                TargetType = typeof(ActionsIndexPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuNavigation"),
                Icon = MaterialDesignIcons.Navigation,
                TargetType = typeof(DashboardsPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuArticle"),
                Icon = MaterialDesignIcons.Note,
                TargetType = typeof(ArticlesPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuSocial"),
                Icon = MaterialDesignIcons.NaturePeople,
                TargetType = typeof(SocialsPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuIcon"),
                Icon = MaterialDesignIcons.InsertEmoticon,
                TargetType = typeof(FontIconsPage)
            },
            new MenuEntry()
            {
                Title = LocalizationResourceManager.Translate("MenuPrivacy"),
                Icon = MaterialDesignIcons.Security,
                TargetType = typeof(PrivacyPolicyPage)
            }
        };
    }

    public List<MenuEntry> MainMenuEntries
    {
        get { return _mainMenuEntries; }
        set { SetProperty(ref _mainMenuEntries, value); }
    }

    public bool IsGridMenuSwitchToggled
    {
        get { return _isGridView; }
        set { SetProperty(ref _isGridView, value); }
    }

    public MenuEntry MainMenuSelectedItem
    {
        get { return _selectedMainMenuEntry; }
        set
        {
            if (SetProperty(ref _selectedMainMenuEntry, value) && value != null)
            {
                NavigationPage navigationPage = new ((Page)Activator.CreateInstance(value.TargetType));

                _openPageAsRoot(navigationPage);

                _selectedMainMenuEntry = null;
                OnPropertyChanged(nameof(MainMenuSelectedItem));
            }
        }
    }

    public ICommand MenuItemSelectionCommand { get; private set; }
    async void SelectedMenuItem(MenuEntry obj)
    {
        MainMenuSelectedItem = obj;
    }

    public class MenuEntry
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public Type TargetType { get; set; }
    }
}
