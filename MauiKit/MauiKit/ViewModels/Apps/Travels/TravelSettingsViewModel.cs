
namespace MauiKit.ViewModels.Travels;
public partial class TravelSettingsViewModel : BaseViewModel
{
    #region Fields
    private INavigation _navigationService;
    private Page _pageService;

    [ObservableProperty]
    public ObservableCollection<MenuItems> _menuItems;
    #endregion Fields

    #region Constructor
    public TravelSettingsViewModel(INavigation navigationService, Page pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;
    }
    #endregion

    #region Commands

    [RelayCommand]
    private async void Goback()
    {
        await _navigationService.PopModalAsync();
    }

    [RelayCommand]
    private void Logout()
    {

    }
    #endregion Commands
}
