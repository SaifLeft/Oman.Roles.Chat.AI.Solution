
namespace MauiKit.Views;

public partial class SocialsPage : BasePage
{
	public SocialsPage()
	{
		InitializeComponent();
	}

    async void ChatHome_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChatHomePage());
    }

    async void ChatDetail_Tapped(object sender, EventArgs e)
    {
        //Get and pass default conversation parameter
        SocialMessage defaultConversation = SocialServices.Instance.GetChats().FirstOrDefault();
        await Navigation.PushAsync(new ChatDetailPage(defaultConversation));
    }

    async void ContactDetail_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ContactDetailPage());
    }

    async void SocialProfile_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SocialProfilePage());
    }

    async void SocialProfileGallery_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SocialProfileGalleryPage());
    }

    async void SocialProfileCard_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SocialProfileCardPage());
    }

    async void SocialProfileBackground_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new SocialProfileBackgroundCoverPage());
    }
}