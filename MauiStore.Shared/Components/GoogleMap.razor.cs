using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MauiStore.Data.GoogleMap;

namespace MauiStore.Shared.Components
{
    public partial class GoogleMap : MudComponentBase, IAsyncDisposable
    {
        public string UniqueID { get; set; }
        private DotNetObjectReference<GoogleMap> reference;

        protected DotNetObjectReference<GoogleMap> Reference
        {
            get
            {
                if (reference == null)
                {
                    reference = DotNetObjectReference.Create(this);
                }

                return reference;
            }
        }
        public ElementReference Element { get; internal set; }
        [Parameter]
        public IEnumerable<GoogleMapMarker> Data { get; set; }

        [Parameter]
        public EventCallback<GoogleMapClickEventArgs> MapClick { get; set; }

        [Parameter]
        public EventCallback<GoogleMapMarker> MarkerClick { get; set; }

        [Parameter]
        public string ApiKey { get; set; } = "AIzaSyDMw54R1Xc0NEYgy3CgAWkdyVrS0IoaYLA"; //Todo: change your Maps API Key here

        [Parameter]
        public double Zoom { get; set; } = 8;

        [Parameter]
        public GoogleMapPosition Center { get; set; } = new GoogleMapPosition() { Lat = 0, Lng = 0 };

        [Parameter]
        public RenderFragment Markers { get; set; }

        [Parameter]
        public virtual bool Visible { get; set; } = true;

        List<GoogleMapMarker> markers = new List<GoogleMapMarker>();
        public void AddMarker(GoogleMapMarker marker)
        {
            if (markers.IndexOf(marker) == -1)
            {
                markers.Add(marker);
            }
        }

        public void RemoveMarker(GoogleMapMarker marker)
        {
            if (markers.IndexOf(marker) != -1)
            {
                markers.Remove(marker);
            }
        }

        protected string Classname => "rz-map";


        [JSInvokable("GoogleMap.OnMapClick")]
        public async System.Threading.Tasks.Task OnMapClick(GoogleMapClickEventArgs args)
        {
            await MapClick.InvokeAsync(args);
        }

        [JSInvokable("GoogleMap.OnMarkerClick")]
        public async System.Threading.Tasks.Task OnMarkerClick(GoogleMapMarker marker)
        {
            await MarkerClick.InvokeAsync(marker);
        }

        protected override void OnInitialized()
        {
            UniqueID = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "-").Replace("+", "-").Substring(0, 10);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var data = Data != null ? Data : markers;

            if (firstRender)
            {
                
                 await _jsRuntime.InvokeVoidAsync("MauiStore.createMap", Element, Reference, UniqueID, ApiKey, Zoom, Center,
                     data.Select(m => new { Title = m.Title, Label = m.Label, Position = m.Position }));
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("MauiStore.updateMap", UniqueID, Zoom, Center,
                             data.Select(m => new { Title = m.Title, Label = m.Label, Position = m.Position }));
            }
        }

        public async Task MoveToCenter()
        {
            var data = Data != null ? Data : markers;
            await _jsRuntime.InvokeVoidAsync("MauiStore.updateMap", UniqueID, Zoom, Center,
                            data.Select(m => new { Title = m.Title, Label = m.Label, Position = m.Position }));
            //await _jsRuntime.InvokeVoidAsync("MauiStore.moveToCenter", UniqueID, Center);
        }
        public ValueTask DisposeAsync()
        {
            return _jsRuntime.InvokeVoidAsync("MauiStore.destroyMap", UniqueID);
        }

        public string GetId()
        {
            if (UserAttributes != null && UserAttributes.TryGetValue("id", out var id) && !string.IsNullOrEmpty(Convert.ToString(@id)))
            {
                return $"{@id}";
            }

            return UniqueID;
        }
    }
}
