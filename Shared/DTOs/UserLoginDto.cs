using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shared.DTOs;


public class UserLoginDto
{
    public string PwdHashed { get; set; } = string.Empty;
    public int Id { get; set; }

    public string AccessToken { get; set; } = string.Empty;

}
