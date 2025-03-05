using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.Measure;

using MauiKit.Controls.Charts;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace MauiKit.ViewModels;
public partial class DashboardVisualViewModel : ObservableObject
{
    public DashboardVisualViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        ChartEntries = new ChartEntry[]
        {
            new ChartEntry
            {
                Value = 71,
                Color = Color.FromArgb("#6023FF"),
                Text = "Visual Studio Code"
            },
            new ChartEntry
            {
                Value = 33,
                Color = Color.FromArgb("#3059FE"),
                Text = "Visual Studio"
            },
            new ChartEntry
            {
                Value = 29,
                Color = Color.FromArgb("#2EF1D2"),
                Text = "Notepad++"
            },
            new ChartEntry
            {
                Value = 28,
                Color = Color.FromArgb("#F8426E"),
                Text = "IntelliJ"
            }
        };
       
    }

    #region TotalUsers Chart
    public ISeries[] TotalUserSeries { get; set; } =
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

    public Axis[] YAxes { get; set; } =
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
            StrokeThickness = 1
        }
    };
    #endregion TotalUsers Chart

    public ChartEntry[] ChartEntries { get; set; }

}
