using Microsoft.AspNetCore.Components;

namespace Frontend.ViewModels
{
    public abstract class ViewModelBase : ComponentBase
    {
        [Inject] protected AppState AppState { get; set; } = default!;
    }
}
