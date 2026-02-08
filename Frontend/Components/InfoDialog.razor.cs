using Microsoft.AspNetCore.Components;

namespace Frontend.Components;

public partial class InfoDialog : ComponentBase
{
    [Parameter] public bool Show { get; set; }

    [Parameter] public string Title { get; set; } = "Info";
    [Parameter] public string Message { get; set; } = "";

    [Parameter] public EventCallback OnClose { get; set; }

    private async Task Close()
        => await OnClose.InvokeAsync();
}
