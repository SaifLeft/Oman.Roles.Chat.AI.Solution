using MauiKit.Models;
using System;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.Measure;

using MauiKit.Controls.Charts;
using LiveChartsCore.Defaults;

namespace MauiKit.ViewModels;
public partial class PolarChartsViewModel : ObservableObject
{
    public PolarChartsViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        //Basic Polar
        BasicPolarSeries = new ISeries[]
        {
            new PolarLineSeries<double>
            {
                Values = new ObservableCollection<double> { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
                DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                GeometrySize = 30,
                DataLabelsSize = 15,
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsRotation = LiveCharts.CotangentAngle,
                IsClosed = true
            }
        };

        //Radial Area Polar
        RadialAreaPolarSeries = new ISeries[]
        {
            new PolarLineSeries<int>
            {
                Values = new[] { 7, 5, 7, 5, 6 },
                LineSmoothness = 0,
                GeometrySize= 0,
                Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90))
            },
            new PolarLineSeries<int>
            {
                Values = new[] { 2, 7, 5, 9, 7 },
                LineSmoothness = 1,
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90))
            }
        };

        //Coordinates Polar
        CoordinatesPolarSeries = new ISeries[]
        {
            new PolarLineSeries<ObservablePolarPoint>
            {
                Values = new[]
                {
                    new ObservablePolarPoint(0, 10),
                    new ObservablePolarPoint(45, 15),
                    new ObservablePolarPoint(90, 20),
                    new ObservablePolarPoint(135, 25),
                    new ObservablePolarPoint(180, 30),
                    new ObservablePolarPoint(225, 35),
                    new ObservablePolarPoint(270, 40),
                    new ObservablePolarPoint(315, 45),
                    new ObservablePolarPoint(360, 50),
                },
                IsClosed = false,
                Fill = null
            }
        };
    }

    //Basic Polar
    public PolarAxis[] BasicPolarRadialAxes { get; set; } =
    {
        new PolarAxis
        {
            LabelsAngle = -60,
            MaxLimit = 30 // null to let the chart autoscale (defualt is null) 
        }
    };

    public PolarAxis[] BasicPolarAngleAxes { get; set; } =
    {
        new PolarAxis
        {
            LabelsRotation = LiveCharts.TangentAngle
        }
    };
    public ISeries[] BasicPolarSeries { get; set; }


    //Radial Area Polar
    public PolarAxis[] RadialAreaPolarAngleAxes { get; set; } =
    {
        new PolarAxis
        {
            LabelsRotation = LiveCharts.TangentAngle,
            Labels = new[] { "first", "second", "third", "forth", "fifth" }
        }
    };

    public ISeries[] RadialAreaPolarSeries { get; set; }


    //Coordinates Polar
    public PolarAxis[] CoordinatesPolarAngleAxes { get; set; } =
    {
        new PolarAxis
        {
            // force the axis to always show 360 degrees.
            MinLimit = 0,
            MaxLimit = 360,
            Labeler = angle => $"{angle}°",
            ForceStepToMin = true,
            MinStep = 30
        }
    };

    public ISeries[] CoordinatesPolarSeries { get; set; }
}
