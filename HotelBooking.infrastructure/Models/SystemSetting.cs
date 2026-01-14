using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class SystemSetting
{
    public string KeyWord { get; set; } = null!;

    public string? SetTime { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdateAt { get; set; }
}
