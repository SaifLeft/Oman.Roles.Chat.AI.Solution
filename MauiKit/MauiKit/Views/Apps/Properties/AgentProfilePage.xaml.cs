namespace MauiKit.Views.Properties;

public partial class AgentProfilePage : BasePage
{
	public AgentProfilePage()
	{
		InitializeComponent();
        BindingContext = new AgentProfileViewModel();
    }

    private async void BackButton_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void CollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView) collectionView.SelectedItem = null;
    }
}