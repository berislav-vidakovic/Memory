using Frontend.Models;
using Frontend.Pages;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs;

namespace Frontend.ViewModels;


/*
MVVM Architecture

-ViewModel owns UI state
-It decides what message to show
-UI just renders whatever ViewModel exposes
*/

public class UsersPageViewModel : ComponentBase
{
    private UserApiService? _userService = default!;
    private UserApiService UserService =>
        _userService ??= InjectedUserService!;

    [Inject]
    public UserApiService? InjectedUserService { get; set; }


    public List<User> AllUsers { get; private set; } = new();

    public bool ShowLoginDialog { get; private set; }
    public bool ShowPwdDialog { get; set; }

    public string? ExistingPassword { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }


    public string? SelectedLogin { get; set; }
    public int? EditUserId{ get; set; }
    public string Password { get; set; } = string.Empty;

    public int? CurrentUserId { get; private set; } = null;

    public event Action? OnStateChanged;


    public UsersPageViewModel(UserApiService? userService = null)
    {
        _userService = userService;
    }

    public async Task LoadUsersAsync()
    {
        AllUsers = await UserService.GetUsersAsync();    
        OnStateChanged?.Invoke();
    }

    public List<User> OfflineUsers =>
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

    public void OpenPwdDialog()
    {
        ShowPwdDialog = true;
        OnStateChanged?.Invoke();
    }

    public void ClosePwdDialog()
    {
        ShowPwdDialog = false;
        OnStateChanged?.Invoke();
    }

    public async Task PwdOk()
    {
        Console.WriteLine("Edit User ID=" + EditUserId);
        ClosePwdDialog();
    }

    public async Task PwdCancel()
    {
        ClosePwdDialog();
    }

    public async Task LoginOk()
    {
        Console.WriteLine("Login attempt login=" + SelectedLogin + " pwd=" + Password);
        User? user = AllUsers.FirstOrDefault(u => u.Login == SelectedLogin);

        if (user == null)
        {
            Console.WriteLine("LOGIN FAILED: No user selected or user not found");
            return;
        }
        Console.WriteLine($"LOGIN: {SelectedLogin}, Password: {Password}, Full name: {user.FullName}");

        string HashedPwd = HashUtil.HashClient(Password);

        UserLoginDto loginBody = new UserLoginDto
        {
            Login = SelectedLogin!,
            PwdHashed = HashedPwd
        };

        bool bSuccess = await UserService.LoginAsync(loginBody);
        if (bSuccess)
            CurrentUserId = user.Id;
        

        Console.WriteLine($"Login result: {bSuccess}");

        CloseLoginDialog();
    }

    public void LoginCancel()
    {
        Console.WriteLine("LOGIN CANCELLED");
        CloseLoginDialog();
    }

    public async Task Logout()
    {
        Console.WriteLine("Logout clicked");
        User? user = AllUsers.FirstOrDefault(u => u.Id == CurrentUserId);

        if (user == null)
        {
            Console.WriteLine("LOGOUT FAILED: No user found");
            return;
        }


        UserLoginDto logoutBody = new UserLoginDto
        {
            Login = user!.Login,
            PwdHashed = ""
        };

        bool bSuccess = await UserService.LogoutAsync(logoutBody);
        if (bSuccess)
            CurrentUserId = null;

        OnStateChanged?.Invoke();
        
    }

}
