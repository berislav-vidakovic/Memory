using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AuthServer.Services;
using Shared.DTOs;
using Shared;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AuthServer.Pages;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    public string? Password { get; set; } = "";

    public List<SelectListItem> Users { get; set; } = new();

    public async Task OnGet()
    {
        var users = await _authService.GetOfflineUsersAsync(); // return List<User>
        // SelectListItem is a helper class in ASP.NET Core used to represent an item in a <select> (dropdown) list
        Users = users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.FullName
        }).ToList();
    }

    public async Task<IActionResult> OnPost()
    {
        Password ??= string.Empty;
        var dto = new UserLoginDto
        {
            Id = Id,            
            PwdHashed = HashUtil.HashClient(Password)
        };

        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        // Set refresh token cookie
        var refreshToken = await _authService.GetRefreshTokenForUser(Id);

        _authService.AppendCookie(
            Response,
            "X-Refresh-Token",
            refreshToken,
            DateTime.UtcNow.AddDays(7));

        // For now: redirect back to SPA
        return Redirect("https://localhost:7173/usersPage");
    }
}
