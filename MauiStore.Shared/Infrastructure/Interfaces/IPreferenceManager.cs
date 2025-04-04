using System.Threading.Tasks;

namespace MauiStore.Infrastructure
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);
        Task<IPreference> GetPreference();
    }
}