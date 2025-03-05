namespace MauiKit.Services.Interfaces;
public interface INavigationService
{
    Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel;
    Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;
    Task NavigateToPageAsync(Page page, object parameter);
    Task<Page> NavigateToPopupAsync<TViewModel>() where TViewModel : BaseViewModel;
    Task<Page> NavigateToPopupAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;
    Task NavigateBackAsync();
    Task RemovePopupAsync();
}

