
namespace MauiKit.ViewModels.Socials;
public partial class ContactDetailViewModel : ObservableObject
{
    public ContactDetailViewModel()
    {
        Avatar = AppSettings.ImageServerPath +  "ecommerce/users/150-13.jpg";
        LoadData();
    }


    #region Methods
    private void LoadData()
    {
        IsBusy = true;
        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(500);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                
                ContactFields = new ObservableCollection<ContactData>(SocialServices.Instance.GetContactFields());

                IsBusy = false;
            });
        });
    }

    #endregion Methods

    #region Public Properties

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _avatar;

    [ObservableProperty]
    private ObservableCollection<ContactData> _contactFields;

    #endregion Public Properties
}
