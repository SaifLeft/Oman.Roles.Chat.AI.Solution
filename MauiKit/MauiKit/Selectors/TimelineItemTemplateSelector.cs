
namespace MauiKit.Selectors;
public class TimelineItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate LeftItemTemplate { get; set; }
    public DataTemplate RightItemTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (((TimelineData)item).IsInbound)
        {
            return LeftItemTemplate;
        }
        else
            return RightItemTemplate;
    }
}
