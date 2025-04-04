using System.Collections.Generic;
using Blazored.LocalStorage;
using MudBlazor;
using System.Threading.Tasks;
using MauiStore.Constants;

namespace MauiStore.Infrastructure
{
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        private readonly ILocalStorageService _localStorageService;
        public ClientPreferenceManager(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        public async Task<bool> IsDarkMode()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                return preference.IsDarkMode;
            }
            return preference.IsDarkMode;
        }
        public async Task<bool> ToggleDarkModeAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsDarkMode = !preference.IsDarkMode;
                await SetPreference(preference);
                return preference.IsDarkMode;
            }

            return false;
        }
        public async Task ChangeFirstVisitAsync(bool isFirstVisit)
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsFirstVisit = isFirstVisit;
                await SetPreference(preference);
            }
        }
        public async Task<bool> IsFirstVisit()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                return preference.IsFirstVisit;
            }
            return preference.IsFirstVisit;
        }
        public async Task<IPreference> GetPreference()
        {
            return await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();
        }
        public async Task SetPreference(IPreference preference)
        {
            await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
        }
    }
}