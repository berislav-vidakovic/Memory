using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs;


public class HealthResponseDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
