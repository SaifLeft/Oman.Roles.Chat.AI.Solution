using PropertyChanged;
using XCalendar.Maui.Models;

namespace MauiKit.Models;

/// <summary>
/// A base class for Models that need to implement the INotifyPropertyChanged interface.
/// </summary>
[AddINotifyPropertyChangedInterface]
public abstract class BaseObservableModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
}

public class ColoredEventsDay : ColoredEventsDay<ColoredEvent>
{
}
public class ColoredEventsDay<TEvent> : CalendarDay<TEvent> where TEvent : IEvent
{
}
