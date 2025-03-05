
namespace MauiKit.Models;
public class DashboardVariantItem
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Avatar { get; set; }
    public Color BackgroundColor { get; set; }
    public string BackgroundImage { get; set; }
    public string Icon { get; set; }
    public int BadgeCount { get; set; }
    public bool ShowBackgroundColor { get; set; }
    public bool IsNotification { get; set; }
    public Type TargetType { get; set; }
}
