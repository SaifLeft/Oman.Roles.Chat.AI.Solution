
using System.Windows.Input;

namespace MauiKit.ViewModels.Properties;
public class PropertySettingsViewModel : BaseViewModel
{
    public ICommand TapCommand { get; private set; }

    public string Name { get; set; } = "Robin Aleson";
    public string Email { get; set; } = "robin.aleson@mauikit.com";
    public string ImageUrl { get; set; } = AppSettings.ImageServerPath +  "ecommerce/users/150-26.jpg";

    public List<MenuItems> _MenuItems = new List<MenuItems>();
    public List<MenuItems> MenuItems
    {
        get { return _MenuItems; }
        set { _MenuItems = value; }
    }

    public PropertySettingsViewModel()
    {
        PopulateData();
        CommandInit();
    }

    void PopulateData()
    {
        MenuItems.Add(new MenuItems() { Title = "Edit Profile", Body = "\uf3eb", TargetType = typeof(PropertyHomePage) });
        MenuItems.Add(new MenuItems() { Title = "Shipping Address", Body = "\uf34e", TargetType = typeof(PropertyHomePage) });
        MenuItems.Add(new MenuItems() { Title = "Wishlist", Body = "\uf2d5", TargetType = typeof(PropertyHomePage) });
        MenuItems.Add(new MenuItems() { Title = "Cards", Body = "\uf19b", TargetType = typeof(PropertyHomePage) });
        MenuItems.Add(new MenuItems() { Title = "Notifications", Body = "\uf09c", TargetType = typeof(PropertyHomePage) });
    }

    private void CommandInit()
    {
        TapCommand = new Command<MenuItems>(item =>
        {
            Application.Current.MainPage.Navigation.PushModalAsync(((Page)Activator.CreateInstance(item.TargetType)));
        });

    }
}
