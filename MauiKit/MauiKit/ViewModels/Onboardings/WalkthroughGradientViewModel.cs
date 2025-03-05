
namespace MauiKit.ViewModels.Onboardings;
public partial class WalkthroughGradientViewModel : BaseViewModel
{
    private INavigation _navigationService;
    private Page _pageService;
    public WalkthroughGradientViewModel(INavigation navigationService, Page pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;

        Boardings = new ObservableCollection<Boarding>();
        CreateBoardingCollection();
    }

    void CreateBoardingCollection()
    {
        Boardings = new ObservableCollection<Boarding>()
        {
            new Boarding
            {
                ImagePath = AppSettings.ImageServerPath + "walkthrough/walkthrough_01_gradient.png",
                Title = AppTranslations.StringWalkthroughTitleStep1,
                Subtitle = AppTranslations.StringWalkthroughSubtitleStep1,
                BackgroundColor1 = Color.FromArgb("#BF3F0041"),
                BackgroundColor2 = Color.FromArgb("#012E8B"),
                BackgroundGradient = new RadialGradientBrush
                    {
                        // Center defaults to (0.5,0,5).
                        Radius = 0.5,
                        GradientStops =
                        {
                            new GradientStop { Color = Color.FromArgb("#BF3F0041"), Offset = 0.1f },
                            new GradientStop { Color = Color.FromArgb("#012E8B"), Offset = 1.0f }
                        }
                    }
            },
            new Boarding
            {
                ImagePath = AppSettings.ImageServerPath + "walkthrough/walkthrough_02_gradient.png",
                Title = AppTranslations.StringWalkthroughTitleStep2,
                Subtitle = AppTranslations.StringWalkthroughSubtitleStep2,
                BackgroundColor1 = Color.FromArgb("#713d74"),
                BackgroundColor2 = Color.FromArgb("#221e60"),
                BackgroundGradient = new RadialGradientBrush
                    {
                        // Center defaults to (0.5,0,5).
                        Radius = 0.5,
                        GradientStops =
                        {
                            new GradientStop { Color = Color.FromArgb("#713d74"), Offset = 0.1f },
                            new GradientStop { Color = Color.FromArgb("#221e60"), Offset = 1.0f }
                        }
                    }
            },
            new Boarding
            {
                ImagePath = AppSettings.ImageServerPath + "walkthrough/walkthrough_03_gradient.png",
                Title = AppTranslations.StringWalkthroughTitleStep3,
                Subtitle = AppTranslations.StringWalkthroughSubtitleStep3,
                BackgroundColor1 = Color.FromArgb("#d54381"),
                BackgroundColor2 = Color.FromArgb("#7644ad"),
                BackgroundGradient = new RadialGradientBrush
                    {
                        // Center defaults to (0.5,0,5).
                        Radius = 0.5,
                        GradientStops =
                        {
                            new GradientStop { Color = Color.FromArgb("#d54381"), Offset = 0.1f },
                            new GradientStop { Color = Color.FromArgb("#7644ad"), Offset = 1.0f }
                        }
                    }
            },
        };
    }

    #region Commands

    [RelayCommand]
    private async void Skip(object obj)
    {
        await CloseWalkThroughPage();
    }

    [RelayCommand]
    private async void Next(object obj)
    {
        if (ValidateAndUpdatePosition())
        {
            await CloseWalkThroughPage();
        }

    }
    #endregion

    #region Methods
    private bool ValidateAndUpdatePosition()
    {
        ValidateSelection(Position + 1);
        if (Position >= Boardings.Count - 1)
            return true;
        Position = Position + 1;
        return false;
    }

    private void ValidateSelection(int index)
    {
        if (index <= Boardings.Count - 2)
        {
            IsSkipButtonVisible = true;
            NextButtonText = AppTranslations.ButtonNext;
        }
        else
        {
            NextButtonText = AppTranslations.ButtonFinish;
            IsSkipButtonVisible = false;
        }
    }

    private async Task CloseWalkThroughPage()
    {
        await _navigationService.PopAsync();
    }

    #endregion

    #region Properties

    [ObservableProperty]
    public ObservableCollection<Boarding> boardings;

    [ObservableProperty]
    private bool isSkipButtonVisible = true;

    [ObservableProperty]
    private int position = 0;

    [ObservableProperty]
    private string nextButtonText = AppTranslations.ButtonNext;

    #endregion
}
