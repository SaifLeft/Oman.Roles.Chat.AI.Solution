
using RGPopup.Maui.Extensions;

namespace MauiKit.Services; 
public class NavigationService : INavigationService
{
    //public async Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel
    //{
    //    var viewModelType = typeof(TViewModel);
    //    var pageType = GetPageTypeForViewModel(viewModelType);
    //    var page = (Page)Activator.CreateInstance(pageType);
    //    await Application.Current.MainPage.Navigation.PushAsync(page);
    //}

    public async Task NavigateBackAsync()
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }

    public Task NavigateToAsync<TViewModel>(bool keepInMasterDetail = true) where TViewModel : BaseViewModel
    {
        return InternalNavigateToAsync(typeof(TViewModel), null, keepInMasterDetail);
    }

    public Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel
    {
        return InternalNavigateToAsync(typeof(TViewModel), null);
    }

    public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel
    {
        return InternalNavigateToAsync(typeof(TViewModel), parameter);
    }
    public async Task<Page> NavigateToPopupAsync<TViewModel>() where TViewModel : BaseViewModel
    {
        return await InternalNavigateToPopupAsync(typeof(TViewModel), null);
    }

    public async Task<Page> NavigateToPopupAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel
    {
        return await InternalNavigateToPopupAsync(typeof(TViewModel), parameter);
    }
    public Task NavigateToPageAsync(Page page, object parameter)
    {
        return InternalNavigateToAsync(page, parameter);
    }

    public async Task RemovePopupAsync()
    {
        INavigation navigation = GetNavigation();
        await navigation.PopPopupAsync();
    }

    private async Task InternalNavigateToAsync(Type viewModelType, object parameter, bool keepInMasterDetail = true)
    {
        Page page = CreatePage(viewModelType, parameter);
        INavigation navigation = GetNavigation(keepInMasterDetail);
        //NavigationPage.SetBackButtonTitle(page, "ABC ");

        await navigation.PushAsync(page);
        await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
    }

    private async Task<Page> InternalNavigateToPopupAsync(Type viewModelType, object parameter)
    {
        Page page = CreatePage(viewModelType, parameter);
        INavigation navigation = GetNavigation();

        await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
        await navigation.PushPopupAsync(page as RGPopup.Maui.Pages.PopupPage);
        return page;
    }

    private async Task InternalNavigateToAsync(Page page, object parameter)
    {
        INavigation navigationPage = GetNavigation();
        await navigationPage.PushAsync(page);
        await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
    }

    private Page CreatePage(Type viewModelType, object parameter)
    {
        Type pageType = GetPageTypeForViewModel(viewModelType);
        if (pageType == null)
        {
            throw new Exception($"Cannot locate page type for {viewModelType}");
        }
        Page page = Activator.CreateInstance(pageType) as Page;
        return page;
    }

    private INavigation GetNavigation(bool keepInMasterDetail = true)
    {
        INavigation navigationPage;
        if (Application.Current.MainPage is FlyoutPage masterDetailPage)
        {
            if (masterDetailPage.Detail is NavigationPage navPage)
            {
                navigationPage = navPage.Navigation;
            }
            else
            {
                var detailNavigationPage = new NavigationPage(masterDetailPage);
                navigationPage = detailNavigationPage.Navigation;
            }
        }
        else
        {
            navigationPage = Application.Current.MainPage.Navigation;
        }
        return navigationPage;
    }

    private Type GetPageTypeForViewModel(Type viewModelType)
    {
        var viewName = viewModelType.FullName.Replace("ViewModel", "Page");
        var viewType = Type.GetType(viewName);
        return viewType;
    }
}
