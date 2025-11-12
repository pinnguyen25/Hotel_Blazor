using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class AccommodationEvent
{
    public int AccommodationTypeId { get; set; }

    public int EventTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Additional { get; set; }

    public virtual AccommodationType AccommodationType { get; set; } = null!;

    public virtual EventType EventType { get; set; } = null!;
}
