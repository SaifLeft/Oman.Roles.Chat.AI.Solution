using System.Linq;

namespace MauiStore.Infrastructure
{
    public record ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; } = false;
        public bool IsFirstVisit { get; set; } = true;
    }
}