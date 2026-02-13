using Microsoft.AspNetCore.Components;

namespace Frontend.ViewModels
{
    public abstract class ViewModelBase : ComponentBase
    {
        [Inject] protected AppState AppState { get; set; } = default!;

        protected override void OnInitialized()
        {
            // services are injected after ctor
            AppState.OnChange += AppStateChanged;

        }
        private void AppStateChanged()
        {
            // This is called when AppState changes
            // Tell Blazor to refresh UI
            InvokeAsync(() => OnAppStateChanged());
        }

        protected virtual void OnAppStateChanged()
        {
            // Raise PropertyChanged or just let Razor component call StateHasChanged
        }

        public void Dispose()
        {
            AppState.OnChange -= AppStateChanged;
        }
    }
}
