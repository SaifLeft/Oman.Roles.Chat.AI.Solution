namespace MauiKit.Views.Socials
{
    public partial class ChatHomePage : BasePage
    {
        ChatHomeViewModel viewModel;
        public ChatHomePage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ChatHomeViewModel(Navigation, this);

            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}