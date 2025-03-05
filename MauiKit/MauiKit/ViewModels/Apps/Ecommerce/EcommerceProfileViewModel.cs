
using MauiKit.Views.Ecommerce;
using System.Windows.Input;

namespace MauiKit.ViewModels.Ecommerce
{
    public class EcommerceProfileViewModel : BaseViewModel
    {
        public ICommand TapCommand { get; private set; }
        public string Name { get; set; } = "Nura Lineon";
        public string Email { get; set; } = "nr-lineon@maui.com";
        public string ImageUrl { get; set; } = AppSettings.ImageServerPath +  "avatars/user2.png";

        public List<MenuItems> _MenuItems = new List<MenuItems>();
        public List<MenuItems> MenuItems
        {
            get { return _MenuItems; }
            set { _MenuItems = value; }
        }

        public EcommerceProfileViewModel()
        {
            PopulateData();
            CommandInit();
        }

        void PopulateData()
        {
            MenuItems.Clear();
            MenuItems.Add(new MenuItems() { Title = "Edit Profile", Icon = IonIcons.Edit, TargetType = null });
            MenuItems.Add(new MenuItems() { Title = "Notifications", Icon = IonIcons.AndroidNotifications, TargetType = null });
            MenuItems.Add(new MenuItems() { Title = "Shipping Address", Icon = IonIcons.Location, TargetType = null });
            MenuItems.Add(new MenuItems() { Title = "Payment Info", Icon = IonIcons.Card, TargetType = null });
            MenuItems.Add(new MenuItems() { Title = "Order History", Icon = IonIcons.AndroidTime, TargetType = null });
            MenuItems.Add(new MenuItems() { Title = "Delete Account", Icon = IonIcons.AndroidDelete, TargetType = null });
        }

        private void CommandInit()
        {
            TapCommand = new Command<MenuItems>(item =>
            {
                if (item.TargetType == null)
                    return;
                Application.Current.MainPage.Navigation.PushAsync(((Page)Activator.CreateInstance(item.TargetType)));
            });
        }
    }
}
