using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs;


public class UsersResponseDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Login { get; set; }
    public bool IsOnline{ get; set; }

}
