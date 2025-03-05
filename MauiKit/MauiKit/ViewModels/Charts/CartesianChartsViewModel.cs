using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;

namespace MauiKit.ViewModels;
public partial class CartesianChartsViewModel : ObservableObject
{
    public CartesianChartsViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        //Basic Bars
        BasicBarXAxes = new Axis[]
        {
            new Axis
            {
                Labels = new string[] { "Category 1", "Category 2", "Category 3" },
                LabelsRotation = 15
            }
        };

        BasicBarSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "Mary",
                Values = new double[] { 2, 5, 4 }
            },
            new ColumnSeries<double>
            {
                Name = "Ana",
                Values = new double[] { 3, 1, 6 }
            }
        };

        //Bars with Background
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
                Values = new double[] { 3, 10, 5, 3, 7, 3, 8 },
                Stroke = null,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                IgnoresBarPosition = true
            }
        };

        //Basic Stacked Area
        BasicStackedAreaSeries = new ISeries[]
        {
            new StackedAreaSeries<double>
            {
                Values = new List<double> { 3, 2, 3, 5, 3, 4, 6 }
            },
            new StackedAreaSeries<double>
            {
                Values = new List<double> { 6, 5, 6, 3, 8, 5, 2 }
            },
            new StackedAreaSeries<double>
            {
                Values = new List<double> { 4, 8, 2, 8, 9, 5, 3 }
            }
        };

        //Stacked Bars
        StackedBarsSeries = new ISeries[]
        {
            new StackedColumnSeries<int>
            {
                Values = new List<int> { 3, 5, -3, 2, 5, -4, -2 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle
            },
            new StackedColumnSeries<int>
            {
                Values = new List<int> { 4, 2, -3, 2, 3, 4, -2 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle
            },
            new StackedColumnSeries<int>
            {
                Values = new List<int> { -2, 6, 6, 5, 4, 3, -2 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle
            }
        };

        //Lines XY
        LinesXYSeries = new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = new ObservablePoint[]
                {
                    new ObservablePoint(0, 4),
                    new ObservablePoint(1, 3),
                    new ObservablePoint(3, 8),
                    new ObservablePoint(18, 6),
                    new ObservablePoint(20, 12)
                }
            }
        };

        //Basic Hear
        BasicHeatXAxes = new Axis[]
        {
            new Axis
            {
                Labels = new[] { "Charles", "Richard", "Ana", "Mari" }
            }
        };

        BasicHeatYAxes = new Axis[]
        {
            new Axis
            {
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" }
            }
        };

        BasicHeatSeries = new ISeries[]
        {
            new HeatSeries<WeightedPoint>
            {
                HeatMap = new[]
                {
                    new SKColor(255, 241, 118).AsLvcColor(), // the first element is the "coldest"
                    SKColors.DarkSlateGray.AsLvcColor(),
                    SKColors.Blue.AsLvcColor() // the last element is the "hottest"
                },
                Values = new ObservableCollection<WeightedPoint>
                {
                    // Charles
                    new(0, 0, 150), // Jan
                    new(0, 1, 123), // Feb
                    new(0, 2, 310), // Mar
                    new(0, 3, 225), // Apr
                    new(0, 4, 473), // May
                    new(0, 5, 373), // Jun

                    // Richard
                    new(1, 0, 432), // Jan
                    new(1, 1, 312), // Feb
                    new(1, 2, 135), // Mar
                    new(1, 3, 78), // Apr
                    new(1, 4, 124), // May
                    new(1, 5, 423), // Jun

                    // Ana
                    new(2, 0, 543), // Jan
                    new(2, 1, 134), // Feb
                    new(2, 2, 524), // Mar
                    new(2, 3, 315), // Apr
                    new(2, 4, 145), // May
                    new(2, 5, 80), // Jun

                    // Mari
                    new(3, 0, 90), // Jan
                    new(3, 1, 123), // Feb
                    new(3, 2, 70), // Mar
                    new(3, 3, 123), // Apr
                    new(3, 4, 432), // May
                    new(3, 5, 142), // Jun
                },
            }
        };
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

    #region Basic Bars
    public Axis[] BasicBarXAxes { get; set; }

    public ISeries[] BasicBarSeries { get; set; }
    
    #endregion Basic Bars

    #region Basic Stacked Area
    public ISeries[] BasicStackedAreaSeries { get; set; }
    
    #endregion Basic Stacked Area

    #region Stacked Bars
    public ISeries[] StackedBarsSeries { get; set; }
    
    #endregion Stacked Bars

    #region Lines XY
    public ISeries[]  LinesXYSeries { get; set; }
    
    #endregion Lines XY

    #region Basic Heat
    public Axis[] BasicHeatXAxes { get; set; }
    public Axis[] BasicHeatYAxes { get; set; }
    public ISeries[] BasicHeatSeries { get; set; }
    
    #endregion Basic Heat
}
