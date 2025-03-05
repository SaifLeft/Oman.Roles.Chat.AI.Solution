namespace MauiKit.ViewModels.Articles;
public partial class AddArticleViewModel : ObservableObject
{
    public AddArticleViewModel() 
    {
        //LoadData();
    }

    public ObservableCollection<SelectedImage> SelectedImages { get; set; } = new ObservableCollection<SelectedImage>();

    #region Methods
    void LoadData()
    {
        IsBusy = true;
        Task.Run(async () =>
        {
            // await api call;
            await Task.Delay(500);
            Application.Current.Dispatcher.Dispatch(() =>
            {
                
                IsBusy = false;
            });
        });
    }

    [RelayCommand]
    public async Task PickImages()
    {
        try
        {
            var selectedImages = await FilePicker.PickMultipleAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Pick images"
            });

            if (selectedImages != null && selectedImages.Any())
            {
                SelectedImages.Clear();

                foreach (var imageFile in selectedImages)
                {
                    // Convert the platform-specific file URI to a displayable path
                    var imagePath = imageFile.FullPath;
                    SelectedImages.Add(new SelectedImage
                    {
                        Title = imageFile.FileName,
                        ImagePath = imagePath,
                    });
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions
        }
    }

    #endregion Methods

    [ObservableProperty]
    private bool _isBusy;

    //[ObservableProperty]
    //private ObservableCollection<SelectedImage> _selectedImages;
}

public class SelectedImage
{
    public string ImagePath { get; set; }
    public Stream ImageStream { get; set; }
    public string Title { get; set; }
}
