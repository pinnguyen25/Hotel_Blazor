using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class HousekeepingTask
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public int RoomId { get; set; }

    public int? StaffId { get; set; }

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public string? Note { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;

    public virtual Staff? Staff { get; set; }
}
