namespace MauiKit.Views.Templates;

public partial class DemoItemTemplate : ContentView
{
	public DemoItemTemplate()
	{
		InitializeComponent();
	}

    /* Header Image */
    public static BindableProperty HeaderImageProperty =
        BindableProperty.Create(
            nameof(HeaderImage),
            typeof(string),
            typeof(DemoItemTemplate),
            string.Empty
        );

    public string HeaderImage
    {
        get { return (string)GetValue(HeaderImageProperty); }
        set { SetValue(HeaderImageProperty, value); }
    }

    /* Header Icon Font Family */
    public static BindableProperty HeaderIconFontFamilyProperty =
        BindableProperty.Create(
            nameof(HeaderIconFontFamily),
            typeof(string),
            typeof(DemoItemTemplate),
            string.Empty
        );

    public string HeaderIconFontFamily
    {
        get { return (string)GetValue(HeaderIconFontFamilyProperty); }
        set { SetValue(HeaderIconFontFamilyProperty, value); }
    }

    /* Header Icon */
    public static BindableProperty HeaderIconProperty =
        BindableProperty.Create(
            nameof(HeaderIcon),
            typeof(string),
            typeof(DemoItemTemplate),
            string.Empty
        );

    public string HeaderIcon
    {
        get { return (string)GetValue(HeaderIconProperty); }
        set { SetValue(HeaderIconProperty, value); }
    }

    /* Item Title */
    public static BindableProperty ItemTitleProperty =
        BindableProperty.Create(
            nameof(ItemTitle),
            typeof(string),
            typeof(DemoItemTemplate),
            string.Empty
        );

    public string ItemTitle
    {
        get { return (string)GetValue(ItemTitleProperty); }
        set { SetValue(ItemTitleProperty, value); }
    }

    /* Tag Label */
    public static BindableProperty TagLabelProperty =
        BindableProperty.Create(
            nameof(TagLabel),
            typeof(string),
            typeof(DemoItemTemplate),
            string.Empty
        );

    public string TagLabel
    {
        get { return (string)GetValue(TagLabelProperty); }
        set { SetValue(TagLabelProperty, value); }
    }

    /* Item Description */
    public static BindableProperty ItemDescriptionProperty =
        BindableProperty.Create(
            nameof(ItemDescription),
            typeof(string),
            typeof(DemoItemTemplate),
            string.Empty
        );

    public string ItemDescription
    {
        get { return (string)GetValue(ItemDescriptionProperty); }
        set { SetValue(ItemDescriptionProperty, value); }
    }
}