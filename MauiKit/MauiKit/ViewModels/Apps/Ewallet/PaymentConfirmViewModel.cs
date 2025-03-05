namespace MauiKit.ViewModels;
public partial class PaymentConfirmViewModel : ObservableObject
{
    public PaymentConfirmViewModel()
    {

    }

    [RelayCommand]
    private async void Confirm()
    {
        await PopupNavigation.Instance.PushAsync(new TransferSuccessPopup());
    }
}
