using Frontend.Pages;
using Frontend.Services;
using Shared.DTOs;
using Shared;
using Frontend.Models;

namespace Frontend.ViewModels;


/*
MVVM Architecture

-ViewModel owns UI state
-It decides what message to show
-UI just renders whatever ViewModel exposes
*/

public class UserViewModel
{
    private readonly UserApiService _userService;    

    public User CurrentUser { get; private set; } = new();

    public UserViewModel(
        UserApiService userService)
    {
        _userService = userService;
    }

}
