using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class BookingRoom
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int RoomTypeId { get; set; }

    public int? RoomId { get; set; }

    public decimal PricePerNight { get; set; }

    public int? SelectedBedTypeId { get; set; }

    public string? GuestName { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Additional { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Room? Room { get; set; }

    public virtual RoomType RoomType { get; set; } = null!;

    public virtual BedType? SelectedBedType { get; set; }
}
