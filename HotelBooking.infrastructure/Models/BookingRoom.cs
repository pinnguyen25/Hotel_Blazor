using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class BookingRoom
{
    public int BookingId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Additional { get; set; }

    public int RoomTypeId { get; set; }

    public int Quantity { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual RoomType RoomType { get; set; } = null!;
}
