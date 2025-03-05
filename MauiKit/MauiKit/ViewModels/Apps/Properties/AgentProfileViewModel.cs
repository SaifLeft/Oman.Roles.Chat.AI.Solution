
namespace MauiKit.ViewModels.Properties;
public partial class AgentProfileViewModel : ObservableObject
{
    public AgentProfileViewModel()
    {
        AgentListings = new ObservableCollection<RealEstateProperty>(RealEstateServices.Instance.GetAgentProperties(1));
        FavoriteListings = new ObservableCollection<RealEstateProperty>(RealEstateServices.Instance.GetAgentProperties(1).Where(x => x.IsFeatured == true));
    }

    #region Methods
    void LoadData()
    {
        AgentListings = new ObservableCollection<RealEstateProperty>(RealEstateServices.Instance.GetAgentProperties(AgentProfile.Id));
    }
    #endregion Methods


    [ObservableProperty] 
    private Agent _agentProfile;

    [ObservableProperty] 
    private ObservableCollection<RealEstateProperty> _agentListings;

    [ObservableProperty]
    private ObservableCollection<RealEstateProperty> _favoriteListings;
}
