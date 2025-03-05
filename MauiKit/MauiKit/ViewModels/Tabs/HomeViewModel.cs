using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;

namespace MauiKit.ViewModels;
public partial class HomeViewModel : ObservableObject
{
    public HomeViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        // Statistics
        BarBackgroundSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                IsHoverable = false, // disables the series from the tooltips 
                Values = new double[] { 10, 10, 10, 10, 10, 10, 10 },
                Stroke = null,
                Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
                IgnoresBarPosition = true
            },
            new ColumnSeries<double>
            {
                IsHoverable = false,
                Values = new double[] { 3, 10, 5, 3, 7, 3, 8 },
                Stroke = null,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                IgnoresBarPosition = true
            }
        };

        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(1000);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                Recommendations = new ObservableCollection<RealEstateProperty>(RealEstateServices.Instance.GetRealEstateProperties().Where(x => x.IsFeatured == true));
                BannerItems = new ObservableCollection<BannerData>(DemoAppServices.Instance.GetBannerItems);
                HotUserTransactions = new ObservableCollection<HomeTransactionData>(DemoAppServices.Instance.GetUserTransactions.Take(5).ToList());
            });
        });
    }

    #region Bars with Background Chart
    public ISeries[] BarBackgroundSeries { get; set; }

    public Axis[] BarBackgroundYAxes { get; set; } =
    {
        new Axis { MinLimit = 0, MaxLimit = 10 }
    };

    public DrawMarginFrame Frame { get; set; } =
    new()
    {
        Fill = new SolidColorPaint
        {
            Color = new SKColor(0, 0, 0, 30)
        },
        Stroke = new SolidColorPaint
        {
            Color = new SKColor(80, 80, 80),
            StrokeThickness = 2
        }
    };

    #endregion Bars with Background Chart

    [ObservableProperty]
    public ObservableCollection<BannerData> _bannerItems;

    [ObservableProperty]
    private ObservableCollection<RealEstateProperty> _recommendations;

    [ObservableProperty]
    public ObservableCollection<HomeTransactionData> _hotUserTransactions;

}
