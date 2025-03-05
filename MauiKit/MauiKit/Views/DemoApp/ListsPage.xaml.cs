namespace MauiKit.Views;

public partial class ListsPage : BasePage
{
	public ListsPage()
	{
		InitializeComponent();
	}

    private async void ListCards_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListCardsPage());
    }
    private async void ListFlat_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListFlatPage());
    }
    private async void ListImage_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListImagePage());
    }
    private async void ListImageRounded_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListImageRoundedPage());
    }
    private async void ListIcon_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListIconPage());
    }
}