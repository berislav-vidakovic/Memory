using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string? Username { get; set; }

    [BindProperty]
    public string? Password { get; set; }

    public IActionResult OnPost()
    {
        // DUMMY for now
        return Redirect("https://localhost:7173/usersPage");
    }
}
