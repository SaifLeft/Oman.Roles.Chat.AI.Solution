using Mohami.AI.Maui.Shared.Infrastructure.Interfaces;
using System.Linq;

namespace Mohami.AI.Maui.Shared.Infrastructure
{
    public record ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; } = false;
        public bool IsFirstVisit { get; set; } = true;
    }
}