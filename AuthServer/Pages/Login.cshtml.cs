using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AuthServer.Services;
using Shared.DTOs;
using Shared;

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
    public string? Password { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var dto = new UserLoginDto
        {
            Id = Id,
            PwdHashed = HashUtil.HashClient("")
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
