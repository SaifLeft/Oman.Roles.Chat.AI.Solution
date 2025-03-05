using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;

namespace MauiKit.ViewModels;
public partial class RemarkViewModel : ObservableObject
{
    public RemarkViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                Recommendations = new ObservableCollection<RealEstateProperty>(RealEstateServices.Instance.GetRealEstateProperties().Where(x => x.IsFeatured == true));

                RecentlyJoinedMembers = new ObservableCollection<MemberData>(DemoAppServices.Instance.GetMembers.Take(5).ToList());
            });
        });
    }

    [ObservableProperty]
    private ObservableCollection<RealEstateProperty> _recommendations;

    [ObservableProperty]
    public ObservableCollection<MemberData> _recentlyJoinedMembers;

}
