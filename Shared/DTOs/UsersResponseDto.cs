using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs;


public class UsersResponseDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string DBmessage { get; set; } = string.Empty;

}
