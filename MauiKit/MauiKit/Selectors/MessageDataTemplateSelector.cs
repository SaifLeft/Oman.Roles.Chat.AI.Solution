
namespace MauiKit.Selectors;

public class MessageDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate SenderMessageTemplate { get; set; }
    public DataTemplate ReceiverMessageTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var message = (TravelMessage)item;

        if (message.Sender == null)
            return ReceiverMessageTemplate;

        return SenderMessageTemplate;
    }
}