
namespace MauiKit.ViewModels.Properties;
public partial class PropertyBookingViewModel : ObservableObject
{
    public PropertyBookingViewModel() 
    {
        LoadData();
    }

    #region Methods
    void LoadData()
    {
        PropertyDetail = RealEstateServices.Instance.GetRealEstateProperties().FirstOrDefault();
    }

    #endregion Methods

    [ObservableProperty]
    private RealEstateProperty _propertyDetail;
}
