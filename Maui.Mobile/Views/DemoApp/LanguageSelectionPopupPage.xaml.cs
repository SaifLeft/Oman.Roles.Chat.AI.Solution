namespace MauiKit.Views;

public partial class LanguageSelectionPopupPage : BasePopupPage
{
    public LanguageSelectionPopupPage()
    {
		InitializeComponent();
        BindingContext = new LanguageSelectionPopupViewModel();
    }
}