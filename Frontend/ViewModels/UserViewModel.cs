using Frontend.Models;
using Frontend.Pages;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs;

namespace Frontend.ViewModels;


public class UserViewModel : ComponentBase
{
    [Inject]
    public UserApiService UserService { get; set; } = default!;
    
    [Parameter]
    public User SelectedUser { get; set; } = new();
        
    [Parameter]
    public string ExistingPassword { get; set; } = string.Empty;

    [Parameter]
    public bool ShowEditUserDialog { get; set; }

    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;


    public event Action? OnStateChanged;

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

    public async Task Ok()
    {
        Console.WriteLine("Edit User Login=" + SelectedUser.Login);
        CloseEditUserDialog();
    }

    public async Task Cancel()
    {
        CloseEditUserDialog();
    }

}

