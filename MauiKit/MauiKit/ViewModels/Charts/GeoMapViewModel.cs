using MauiKit.Models;
using System;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.Geo;

namespace MauiKit.ViewModels;
public partial class GeoMapViewModel : ObservableObject
{
    public bool _isBrazilInChart = true;
    public HeatLand _brazil;
    public Random _r = new();

    public GeoMapViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        //Basic GeoMap
        BasicGeoMapSeries = new HeatLandSeries[]
        {
            new HeatLandSeries
            {
                // every country has a unique identifier
                // check the "shortName" property in the following
                // json file to assign a value to a country in the heat map
                // https://github.com/beto-rodriguez/LiveCharts2/blob/master/docs/_assets/word-map-index.json
                Lands = new HeatLand[]
                {
                    new HeatLand { Name = "bra", Value = 13 },
                    new HeatLand { Name = "mex", Value = 10 },
                    new HeatLand { Name = "usa", Value = 15 },
                    new HeatLand { Name = "deu", Value = 13 },
                    new HeatLand { Name = "fra", Value = 8 },
                    new HeatLand { Name = "kor", Value = 10 },
                    new HeatLand { Name = "zaf", Value = 12 },
                    new HeatLand { Name = "are", Value = 13 }
                }
            }
        };

        //World Heat GeoMap
        // every country has a unique identifier
        // check the "shortName" property in the following
        // json file to assign a value to a country in the heat map
        // https://github.com/beto-rodriguez/LiveCharts2/blob/master/docs/_assets/word-map-index.json
        
        var lands = new HeatLand[]
        {
            new() { Name = "bra", Value = 13 },
            new() { Name = "mex", Value = 10 },
            new() { Name = "usa", Value = 15 },
            new() { Name = "can", Value = 8 },
            new() { Name = "ind", Value = 12 },
            new() { Name = "deu", Value = 13 },
            new() { Name= "jpn", Value = 15 },
            new() { Name = "chn", Value = 14 },
            new() { Name = "rus", Value = 11 },
            new() { Name = "fra", Value = 8 },
            new() { Name = "esp", Value = 7 },
            new() { Name = "kor", Value = 10 },
            new() { Name = "zaf", Value = 12 },
            new() { Name = "are", Value = 13 }
        };

        WorldHeatGeoMapSeries = new HeatLandSeries[] { new HeatLandSeries { Lands = lands } };

        _brazil = lands.First(x => x.Name == "bra");
        DoRandomChanges();
    }

    //Basic GeoMap
    public HeatLandSeries[] BasicGeoMapSeries { get; set; }

    //World Heat GeoMap
    public HeatLandSeries[] WorldHeatGeoMapSeries { get; set; }

    [RelayCommand]
    public void ToggleBrazil()
    {
        var lands = WorldHeatGeoMapSeries[0].Lands;
        if (lands is null) return;

        if (_isBrazilInChart)
        {
            WorldHeatGeoMapSeries[0].Lands = lands.Where(x => x != _brazil).ToArray();
            _isBrazilInChart = false;
            return;
        }

        WorldHeatGeoMapSeries[0].Lands = [.. lands, _brazil];
        _isBrazilInChart = true;
    }

    private async void DoRandomChanges()
    {
        await Task.Delay(1000);

        while (true)
        {
            foreach (var shape in WorldHeatGeoMapSeries[0].Lands ?? Enumerable.Empty<IWeigthedMapLand>())
            {
                shape.Value = _r.Next(0, 20);
            }

            await Task.Delay(500);
        }
    }
}
