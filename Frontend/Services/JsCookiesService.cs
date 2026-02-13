using Microsoft.JSInterop;

namespace Frontend.Services;
public class JsCookiesService
{
    private readonly IJSRuntime _js;

    public JsCookiesService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<T?> PostAsync<T>(string url, object body)
    {
        return await _js.InvokeAsync<T>(
            "sendRequestPOST",
            url,
            body
        );
    }
}
