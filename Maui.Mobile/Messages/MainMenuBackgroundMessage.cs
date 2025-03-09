
namespace MauiKit.Messages;

public class MainMenuBackgroundSourceMessage : ValueChangedMessage<string>
{
    public MainMenuBackgroundSourceMessage(string value) : base(value)
    {
    }
}
