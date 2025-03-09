
namespace MauiKit.ViewModels;
public partial class PrivacyPolicyViewModel : ObservableObject
{
    [ObservableProperty]
    string _url;
    public PrivacyPolicyViewModel()
    {
        Url = "http://tlssoftwarevn.com/mauikit-privacy.html";
    }
}
