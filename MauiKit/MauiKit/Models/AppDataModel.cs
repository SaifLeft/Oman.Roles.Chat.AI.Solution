namespace MauiKit.Models;

public class HomeTransactionData
{
    public string ImageIcon { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Status { get; set; }
    public double Amount { get; set; }
    public bool IsCredited { get; set; }
}

public class NewAnnouncementData
{
    public string CoverImage { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}

public class MemberData
{
    public string Avatar { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Position { get; set; }
}

public class BannerData
{
    public string Icon { get; set; }
    public string BackgroundImage { get; set; }
    public Color BackgroundColor { get; set; }
    public Color BackgroundGradientStart { get; set; }
    public Color BackgroundGradientEnd { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Status { get; set; }
}
