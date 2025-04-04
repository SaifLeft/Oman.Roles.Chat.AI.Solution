using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MauiStore.Shared.Components
{
    public partial class BottomNavMenu : MudComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool ShowMenuTitle { get; set; } = true;
    }
}
