
namespace MauiKit.ViewModels.Socials;
public partial class SocialProfileViewModel : ObservableObject
{
    public SocialProfileViewModel() 
    {
        Name = "Feyza Yildirim";
        Image = AppSettings.ImageServerPath +  "social/profile_image_2.jpg";
        Description = "What is on your mind? Share it with the world.";
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _image;
}
