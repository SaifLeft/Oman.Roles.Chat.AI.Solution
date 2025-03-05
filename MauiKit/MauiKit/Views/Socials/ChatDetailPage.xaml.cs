namespace MauiKit.Views.Socials
{
    public partial class ChatDetailPage : BasePage
    {
        ChatDetailViewModel viewModel;
        public ChatDetailPage(SocialMessage selectedConversation)
        {
            InitializeComponent();
            BindingContext = viewModel = new ChatDetailViewModel(Navigation, selectedConversation);

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnBackButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }
    }
}