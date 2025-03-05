namespace MauiKit.Views.Travels;
public partial class ProfileUpdatePage : PopupPage
{
    public ProfileUpdatePage()
    {
        InitializeComponent();
    }

    private async void SubmitButton_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }

    private async void ChangeAvatar_Tapped(object sender, TappedEventArgs e)
    {
        //var options = new StoreCameraMediaOptions { CompressionQuality = 80 };
        //var result = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
        //{
        //    CompressionQuality = 92
        //});
        //if (result is null) return;

        //profileImage.Source = result?.Path;

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick Image",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
            return;

        var stream = await result.OpenReadAsync();
        profileImage.Source = ImageSource.FromStream(() => stream);
    }
}
