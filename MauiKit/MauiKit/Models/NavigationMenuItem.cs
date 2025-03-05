namespace MauiKit.Models;

public class NavigationMenuItem
{
    public string Title { get; set; }
    public NavigationMenuIconItem FontImageIcon { get; set; }
    public Type TargetType { get; set; }
}

public class NavigationMenuIconItem
{
    public string Icon { get; set; }
    public Color IconColor { get; set; }
}
