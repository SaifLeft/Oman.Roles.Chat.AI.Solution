using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.Defaults;

namespace MauiKit.ViewModels;
public partial class PieChartsViewModel : ObservableObject
{
    public PieChartsViewModel()
    {
        LoadData();
    }
    
    public void LoadData()
    {

        //Basic Pie
        BasicPieSeries = new[] { 8, 6, 5, 3, 3 }.AsPieSeries();

        //Donut
        DonutPieSeries = new ISeries[]
        {
            new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 80 }
        };

        //Custom
        CustomPieSeries = new ISeries[]
        {
            new PieSeries<double> { Values = new List<double> { 4 }, OuterRadiusOffset = 0.60 },
            new PieSeries<double> { Values = new List<double> { 5 }, OuterRadiusOffset = 0.65 },
            new PieSeries<double> { Values = new List<double> { 3 }, OuterRadiusOffset = 0.70 },
            new PieSeries<double> { Values = new List<double> { 5 }, OuterRadiusOffset = 0.85 },
            new PieSeries<double> { Values = new List<double> { 7 }, OuterRadiusOffset = 1.00 },
        };

        //Nightingale Rose
        NightingaleRosePieSeries = new ISeries[]
        {
            new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 50, OuterRadiusOffset = 1.0 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, OuterRadiusOffset = 0.9 },
            new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 50, OuterRadiusOffset = 0.8 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, OuterRadiusOffset = 0.7 },
            new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 50, OuterRadiusOffset = 0.6 }
        };

        //Basic Gauge
        BasicGaugePieSeries = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(
                30,          // the gauge value
                series =>    // the series style
                {
                    series.MaxRadialColumnWidth = 50;
                    series.DataLabelsSize = 50;
                }));

        //270 Degrees Gauge
        DegreesGaugePieSeries = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.YellowGreen);
                series.DataLabelsSize = 50;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.Red);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 75;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));

        //Multiple Values Gauge
        MultipleValuesGaugePieSeries = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series => SetMultipleValuesGaugeStyle("Vanessa", series)),
            new GaugeItem(50, series => SetMultipleValuesGaugeStyle("Charles", series)),
            new GaugeItem(70, series => SetMultipleValuesGaugeStyle("Ana", series)),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 20;
            }));

        //Slim Gauge
        SlimGaugePieSeries = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(50, series => SetSlimGaugeStyle("Vanessa", series)),
                new GaugeItem(80, series => SetSlimGaugeStyle("Charles", series)),
                new GaugeItem(95, series => SetSlimGaugeStyle("Ana", series)),
                new GaugeItem(GaugeItem.Background, series =>
                {
                    series.Fill = null;
                }));

    }

    public static void SetMultipleValuesGaugeStyle(string name, PieSeries<ObservableValue> series)
    {
        series.Name = name;
        series.DataLabelsPosition = PolarLabelsPosition.Start;
        series.DataLabelsFormatter =
                point => $"{point.Coordinate.PrimaryValue} {point.Context.Series.Name}";
        series.InnerRadius = 20;
        series.RelativeOuterRadius = 8;
        series.RelativeInnerRadius = 8;
    }

    public static void SetSlimGaugeStyle(string name, PieSeries<ObservableValue> series)
    {
        series.Name = name;
        series.DataLabelsSize = 20;
        series.DataLabelsPosition = PolarLabelsPosition.End;
        series.DataLabelsFormatter =
                point => point.Coordinate.PrimaryValue.ToString();
        series.InnerRadius = 20;
        series.MaxRadialColumnWidth = 5;
    }

    #region Pie Chart
    public IEnumerable<ISeries> BasicPieSeries { get; set; }
    public ISeries[] DonutPieSeries { get; set; }
    public ISeries[] CustomPieSeries { get; set; }
    public ISeries[] NightingaleRosePieSeries { get; set; }
    public IEnumerable<ISeries> BasicGaugePieSeries { get; set; }
    public IEnumerable<ISeries> DegreesGaugePieSeries { get; set; }
    public IEnumerable<ISeries> MultipleValuesGaugePieSeries { get; set; }
    public IEnumerable<ISeries> SlimGaugePieSeries { get; set; }

    #endregion Pie Chart
}
