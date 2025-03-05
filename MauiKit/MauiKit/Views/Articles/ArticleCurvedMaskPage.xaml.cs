namespace MauiKit.Views.Articles;

public partial class ArticleCurvedMaskPage : BasePage
{
	public ArticleCurvedMaskPage()
	{
		InitializeComponent();
		BindingContext = new ArticleCurvedMaskViewModel();

    }

    private void OnAddComment_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Button Tapped", "Add Comment", "OK");
    }
}