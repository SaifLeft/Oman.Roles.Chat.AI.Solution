
namespace MauiKit.ViewModels.Properties;
public partial class PropertyMapViewModel : ObservableObject
{

    public PropertyMapViewModel()
    {
        LoadData();
    }

    #region Methods
    void LoadData()
    {
        Recommendations = new ObservableCollection<RealEstateProperty>(RealEstateServices.Instance.GetRealEstateProperties());
    }
    #endregion Methods

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<RealEstateProperty> _recommendations;

}
