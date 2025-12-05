using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class RoomBedType
{
    public int RoomTypeId { get; set; }

    public int BedTypeId { get; set; }

    public int Quantity { get; set; }

    public bool? IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Additional { get; set; }

    public virtual BedType BedType { get; set; } = null!;

    public virtual RoomType RoomType { get; set; } = null!;
}
