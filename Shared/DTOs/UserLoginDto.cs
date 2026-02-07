using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shared.DTOs;


public class UserLoginDto
{
    public required string PwdHashed { get; set; }
    public required string Login { get; set; }

}
