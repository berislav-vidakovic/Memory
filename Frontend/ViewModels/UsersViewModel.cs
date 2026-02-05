using Frontend.Pages;
using Frontend.Services;
using Shared.DTOs;

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

    public string? SelectedUserFullName { get; set; }
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
        SelectedUserFullName = null;
        OnStateChanged?.Invoke();
    }

    public void LoginOk()
    {
        Console.WriteLine($"LOGIN OK: {SelectedUserFullName}, Password: {Password}");
        CloseLoginDialog();
    }

    public void LoginCancel()
    {
        Console.WriteLine("LOGIN CANCELLED");
        CloseLoginDialog();
    }

}
