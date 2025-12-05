using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class RoomViewType
{
    public int RoomTypeId { get; set; }

    public int ViewTypeId { get; set; }

    public bool? IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Additional { get; set; }

    public virtual RoomType RoomType { get; set; } = null!;

    public virtual ViewType ViewType { get; set; } = null!;
}
