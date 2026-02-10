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
    [Inject]
    public UserApiService UserService { get; set; } = default!;

    public List<User> AllUsers { get; private set; } = new();

    public bool ShowLoginDialog { get; private set; }
    public bool ShowUserDialog { get; set; }


    public int? SelectedId { get; set; }
    public int? EditUserId{ get; set; }
    public string Password { get; set; } = string.Empty;

    public int? CurrentUserId { get; set; } = null;

    public event Action? OnStateChanged;
    

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
        SelectedId = null;
        OnStateChanged?.Invoke();
    }

    public void OpenPwdDialog()
    {
        ShowUserDialog = true;
        OnStateChanged?.Invoke();
    }

    public void ClosePwdDialog()
    {
        ShowUserDialog = false;
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
        Console.WriteLine("Login attempt login=" + SelectedId + " pwd=" + Password);
        User? user = AllUsers.FirstOrDefault(u => u.Id == SelectedId);

        if (user == null)
        {
            Console.WriteLine("LOGIN FAILED: No user selected or user not found");
            return;
        }
        Console.WriteLine($"LOGIN: {SelectedId}, Password: {Password}, Full name: {user.FullName}");

        Console.WriteLine("Entered pwd: " + Password);
        string HashedPwd = HashUtil.HashClient(Password);
        Console.WriteLine("ClientHashed  pwd: " + HashedPwd);

        UserLoginDto loginBody = new UserLoginDto
        {
            Id = (int)SelectedId!,
            PwdHashed = HashedPwd
        };

        bool bSuccess = await UserService.LoginAsync(loginBody);
        if (bSuccess)
        {
            CurrentUserId = user.Id;
            user.IsOnline = true;
        }
        

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
            Id = user!.Id,
            PwdHashed = ""
        };

        bool bSuccess = await UserService.LogoutAsync(logoutBody);
        if (bSuccess)
        {
            CurrentUserId = null;
            user.IsOnline = false;
        }

        OnStateChanged?.Invoke();
        
    }

}
