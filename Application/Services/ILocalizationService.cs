namespace Application.Services
{
    /// <summary>
    /// Service interface for localization operations
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Get a localized message
        /// </summary>
        /// <param name="key">The message key</param>
        /// <param name="section">The section containing the message</param>
        /// <param name="language">The language code</param>
        /// <returns>The localized message</returns>
        string GetMessage(string key, string section, string language);
    }
}