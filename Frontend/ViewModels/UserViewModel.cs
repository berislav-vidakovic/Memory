using Frontend.Models;
using Frontend.Pages;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs;

namespace Frontend.ViewModels;


public class UserViewModel : ViewModelBase
{
    [Inject]
    public UserApiService UserService { get; set; } = default!;

    #region Parameters
    [Parameter]
    public User SelectedUser { get; set; } = new();



    [Parameter]
    public bool ShowEditUserDialog { get; set; }


    #endregion



    public event Action? OnStateChanged;

    protected override void OnAppStateChanged()
    {
        OnStateChanged?.Invoke();
    }


    public void OpenEditUserDialog()
    {
        ShowEditUserDialog = true;
        OnStateChanged?.Invoke();
    }

    public void CloseEditUserDialog()
    {
        ShowEditUserDialog = false;
        OnStateChanged?.Invoke();
    }

    

}

