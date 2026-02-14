using AuthServer.Services;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Models
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty] public required string Username { get; set; }
        [BindProperty] public required string Password { get; set; }

        public IActionResult OnPost(string? returnUrl)
        {
            Console.WriteLine($"Dummy login attempt: {Username}");

            return Page();
        }
    }
}
