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

    public string StatusMessage { get; private set; } = "Loading users...";

    public UsersResponseDto? UsersAll { get; private set; }

    public event Action? OnStateChanged;


    public UsersViewModel(
        UserApiService userService)
    {
        _userService = userService;
    }

    public async Task LoadUsersAsync()
    {
        UsersAll = await _userService.GetUsersAsync();

        if (UsersAll != null)
        {
            StatusMessage = UsersAll.Status;
        }
        else
            StatusMessage = "(no users available)";


        OnStateChanged?.Invoke();
    }

}
