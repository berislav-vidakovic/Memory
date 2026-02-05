using Frontend.Pages;
using Frontend.Services;
using Shared.DTOs;
using Shared;

namespace Frontend.ViewModels;


/*
MVVM Architecture

-ViewModel owns UI state
-It decides what message to show
-UI just renders whatever ViewModel exposes
*/

public class UsersViewModel
{
    private readonly UserApiService _userService;    

    public List<UsersResponseDto> AllUsers { get; private set; } = new();

    public bool ShowLoginDialog { get; private set; }

    public string? SelectedLogin { get; set; }
    public string Password { get; set; } = string.Empty;


    public event Action? OnStateChanged;


    public UsersViewModel(
        UserApiService userService)
    {
        _userService = userService;
    }

    public async Task LoadUsersAsync()
    {
        AllUsers = await _userService.GetUsersAsync();    
        OnStateChanged?.Invoke();
    }

    public List<UsersResponseDto> OfflineUsers =>
        AllUsers.Where(u => !u.IsOnline).ToList();

    public void OpenLoginDialog()
    {
        ShowLoginDialog = true;
        OnStateChanged?.Invoke();
    }

    public void CloseLoginDialog()
    {
        ShowLoginDialog = false;
        Password = string.Empty;
        SelectedLogin = null;
        OnStateChanged?.Invoke();
    }

    public async Task LoginOk()
    {
        var user = AllUsers.FirstOrDefault(u => u.Login == SelectedLogin);

        if (user == null)
        {
            Console.WriteLine("LOGIN FAILED: No user selected or user not found");
            return;
        }
        Console.WriteLine($"LOGIN: {SelectedLogin}, Password: {Password}, Full name: {user.FullName}");

        string HashedPwd = Utils.HashPassword(Password);

        UserLoginDto loginBody = new UserLoginDto
        {
            Login = SelectedLogin,
            PwdHashed = HashedPwd
        };

        bool loginSuccesss = await _userService.LoginAsync(loginBody);

        Console.WriteLine($"Login result: {loginSuccesss}");

        CloseLoginDialog();
    }

    public void LoginCancel()
    {
        Console.WriteLine("LOGIN CANCELLED");
        CloseLoginDialog();
    }

}
