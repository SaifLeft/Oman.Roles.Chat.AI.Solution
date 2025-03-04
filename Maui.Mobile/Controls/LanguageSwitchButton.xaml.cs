using Maui.Service;

namespace Maui.Mobile.Controls;

public partial class LanguageSwitchButton : ContentView
{
    private readonly ILocalizationService _localizationService;

    public LanguageSwitchButton()
    {
        InitializeComponent();

        // Get the localization service from the DI container
        _localizationService = Handler.MauiContext?.Services.GetService<ILocalizationService>();

        // Set initial button text based on current language
        UpdateButtonText();
    }

    private async void OnLanguageSwitchClicked(object sender, EventArgs e)
    {
        if (_localizationService == null)
            return;

        // Get current language
        string currentLanguage = await _localizationService.GetCurrentLanguageAsync();

        // Toggle between Arabic and English
        string newLanguage = currentLanguage == "ar" ? "en" : "ar";

        // Set the new language
        await _localizationService.SetLanguageAsync(newLanguage);

        // Update button text
        UpdateButtonText();
    }

    private async void UpdateButtonText()
    {
        if (_localizationService == null)
            return;

        string currentLanguage = await _localizationService.GetCurrentLanguageAsync();
        SwitchLanguageButton.Text = currentLanguage == "ar" ? "EN" : "ÚÑÈí";
    }
}