
using System.Windows.Input;

namespace MauiKit.ViewModels.Properties;

public partial class PropertyDetailViewModel : ObservableObject
{
    public PropertyDetailViewModel()
    {
        LoadData();
    }

    #region Methods
    void LoadData()
    {
        RealEstateProperty = RealEstateServices.Instance.GetRealEstateProperties().FirstOrDefault();
    }
    #endregion Methods

    [ObservableProperty]
    private RealEstateProperty _realEstateProperty;
}
