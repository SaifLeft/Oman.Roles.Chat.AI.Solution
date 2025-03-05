
namespace MauiKit.ViewModels;
public partial class MyCardsViewModel : ObservableObject
{
    public MyCardsViewModel()
    {
        LoadData();
    }

    void LoadData()
    {
        MyCardLists = new ObservableCollection<CardData>(EwalletServices.Instance.GetMyCards);
    }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    public ObservableCollection<CardData> _myCardLists;

    [RelayCommand]
    private async void CardOptions()
    {
        await PopupNavigation.Instance.PushAsync(new CardOptionsPopup());
    }

    [RelayCommand]
    private async void AddNewCard()
    {
        await PopupNavigation.Instance.PushAsync(new NewCardPopup());
    }
}
