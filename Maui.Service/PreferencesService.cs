namespace Maui.Service
{
    public interface IPreferencesService
    {
        Task SaveValueAsync(string key, string value);
        Task<string> GetValueAsync(string key);
        Task<bool> ContainsKeyAsync(string key);
        Task RemoveValueAsync(string key);
        Task ClearAsync();
    }

    public class PreferencesService : IPreferencesService
    {
        public Task SaveValueAsync(string key, string value)
        {
            Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }

        public Task<string> GetValueAsync(string key)
        {
            return Task.FromResult(Preferences.Default.Get(key, string.Empty));
        }

        public Task<bool> ContainsKeyAsync(string key)
        {
            return Task.FromResult(Preferences.Default.ContainsKey(key));
        }

        public Task RemoveValueAsync(string key)
        {
            Preferences.Default.Remove(key);
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            Preferences.Default.Clear();
            return Task.CompletedTask;
        }
    }
}