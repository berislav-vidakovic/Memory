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

    #region Parameters
    [Parameter]
    public User SelectedUser { get; set; } = new();
        
    [Parameter]
    public string ExistingPassword { get; set; } = string.Empty;

    [Parameter]
    public bool ShowEditUserDialog { get; set; }
    #endregion

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

        Console.WriteLine("Existing Password : " + ExistingPassword);
        Console.WriteLine("Entered Password : " + CurrentPassword);
        //CloseEditUserDialog();

        //return;

        if ( !ValidateCurrentPassword() )
        {
            Console.WriteLine("Invalid password");
        }
        else
        {
            Console.WriteLine("VALID password");

        }




        CloseEditUserDialog();
    }

    private bool ValidateCurrentPassword()
    {

        string currPwd = HashUtil.HashClient(CurrentPassword);
        bool result = HashUtil.VerifyPasswordServer(currPwd, ExistingPassword);
        return result;
    }

    public async Task Cancel()
    {
        CloseEditUserDialog();
    }

}

