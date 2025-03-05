
namespace MauiKit.ViewModels.Socials;
public partial class SocialProfileBackgroundCoverViewModel : ObservableObject
{
    public SocialProfileBackgroundCoverViewModel() 
    {
        LoadData();
    }

    #region Methods
    void LoadData()
    {
        IsBusy = true;
        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                Profile = SocialServices.Instance.GetProfile();

                IsBusy = false;
            });
        });
    }
    #endregion Methods

    #region Public Properties

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ProfileData _profile;

    #endregion Public Properties
}
