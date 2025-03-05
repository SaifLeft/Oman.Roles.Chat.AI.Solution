
namespace MauiKit.Views;

public partial class StatisticPage : ContentPage
{
    public StatisticPage()
	{
		InitializeComponent();
        BindingContext = new StatisticViewModel();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    
}

