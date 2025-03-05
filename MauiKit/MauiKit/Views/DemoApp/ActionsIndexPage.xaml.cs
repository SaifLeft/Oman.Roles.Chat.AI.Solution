
namespace MauiKit.Views;

public partial class ActionsIndexPage : BasePage
{
	public ActionsIndexPage()
	{
        InitializeComponent();
    }

    //Welcome Back
    private async void ActionWelcomeBack_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionWelcomePopupPage());
    }

    //Password Reset Successful
    private async void ActionPasswordReset_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionPasswordResetPopupPage());
    }

    //Payment Failed
    private async void ActionPaymentFailed_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionPaymentFailedPopupPage());
    }

    //Congratulation Confirmation
    private async void ActionCongratulation_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionCongratulationPopupPage());
    }

    //Share Grid
    private async void ShareGrid_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionShareGridPopup());
    }

    //Share List
    private async void ShareList_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionShareListPopup());
    }

    //Shipping Info Box
    private async void ShippingInfo_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ShippingInfoPopupPage());
    }

    //Payment Method Box
    private async void PaymentMethod_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new PaymentMethodPopupPage());
    }

    //Call Box
    private async void CallButton_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionCallPopup());
    }

    //Confirmation Box
    private async void Confirmation_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionConfirmationPopup());
    }

    //Success Box
    private async void SuccessBox_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionSuccessPopup());
    }

    //Success Box with Background
    private async void SuccessBoxBackground_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionSuccessBackgroundPopup());
    }

    //Warning Box
    private async void WarningBox_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionWarningPopup());
    }

    //Warning Box with Background
    private async void WarningBoxBackground_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionWarningBackgroundPopup());
    }

    //Survey Questions Box
    private async void SurveyQuestions_Tapped(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PushAsync(new ActionSurveysPopup());
    }

}