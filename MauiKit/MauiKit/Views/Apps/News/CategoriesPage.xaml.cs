namespace MauiKit.Views.News;

public partial class CategoriesPage : BasePage
{
	public CategoriesPage()
	{
		InitializeComponent();
		this.BindingContext = new CategoriesViewModel();
	}
}
