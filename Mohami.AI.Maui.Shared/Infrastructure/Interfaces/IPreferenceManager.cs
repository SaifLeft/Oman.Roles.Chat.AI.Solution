using System.Threading.Tasks;

namespace Mohami.AI.Maui.Shared.Infrastructure.Interfaces
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);
        Task<IPreference> GetPreference();
    }
}