using IImage = Microsoft.Maui.IImage;

namespace MauiKit.Controls.TintMask;
public interface ITintColorMask : IImage
{
    /// <summary>
    /// Tint color of an image.
    /// </summary>
    Color TintColor { get; set; }
}
