using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiKit.ViewModels.Properties;

public partial class PropertyListingCardViewModel : ObservableObject
{
    public PropertyListingCardViewModel()
    {
        LoadData();
    }

    #region Methods
    void LoadData()
    {
        Categories = new ObservableCollection<Category>(RealEstateServices.Instance.GetCategories);
        Recommendations = new ObservableCollection<RealEstateProperty>(RealEstateServices.Instance.GetRealEstateProperties());
    }
    #endregion Methods

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<Category> _categories;

    [ObservableProperty]
    private ObservableCollection<RealEstateProperty> _recommendations;

}
