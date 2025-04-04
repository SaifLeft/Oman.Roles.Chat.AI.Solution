using MudBlazor;
using System.Threading.Tasks;

namespace Mohami.AI.Maui.Shared.Infrastructure.Interfaces
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<bool> ToggleDarkModeAsync();
        Task ChangeFirstVisitAsync(bool isFirstVisit);
    }
}