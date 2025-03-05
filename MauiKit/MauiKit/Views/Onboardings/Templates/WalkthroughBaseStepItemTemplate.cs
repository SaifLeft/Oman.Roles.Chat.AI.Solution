

namespace MauiKit.Views.Onboardings;
public class WalkthroughBaseStepItemTemplate : ContentView
{
    public event EventHandler ItemAppearing;
    public event EventHandler ItemDisappearing;
    public event EventHandler ItemInitialized;

    public void Initialize()
    {
        ItemInitialized?.Invoke(this, EventArgs.Empty);
    }

    public void ItemAppear()
    {
        ItemAppearing?.Invoke(this, EventArgs.Empty);
    }

    public void ItemDisappear()
    {
        ItemDisappearing?.Invoke(this, EventArgs.Empty);
    }

    /* TEXT */

    public static BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(WalkthroughBaseStepItemTemplate),
            string.Empty
        );

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    /* HEADER */

    public static BindableProperty HeaderProperty =
        BindableProperty.Create(
            nameof(Header),
            typeof(string),
            typeof(WalkthroughBaseStepItemTemplate),
            string.Empty
        );

    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    /* IMAGE */

    public static BindableProperty StepImageProperty =
        BindableProperty.Create(
            nameof(StepImage),
            typeof(string),
            typeof(WalkthroughBaseStepItemTemplate),
            string.Empty
        );

    public string StepImage
    {
        get { return (string)GetValue(StepImageProperty); }
        set { SetValue(StepImageProperty, value); }
    }
}

