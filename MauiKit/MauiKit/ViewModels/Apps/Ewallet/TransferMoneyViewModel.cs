namespace MauiKit.ViewModels;
public partial class TransferMoneyViewModel : ObservableObject
{
    public TransferMoneyViewModel()
    {

    }

    [RelayCommand]
    private async void Transfer()
    {
        await PopupNavigation.Instance.PushAsync(new TransferSuccessPopup());
    }
}
