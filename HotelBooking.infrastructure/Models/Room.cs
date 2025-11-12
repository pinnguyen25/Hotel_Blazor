using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Room
{
    public int Id { get; set; }

    public int RoomTypeId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string? Status { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Additional { get; set; }

    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual RoomType RoomType { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
