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

}
