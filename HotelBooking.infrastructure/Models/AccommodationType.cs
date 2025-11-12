using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class AccommodationType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Additional { get; set; }

    public virtual ICollection<AccommodationEvent> AccommodationEvents { get; set; } = new List<AccommodationEvent>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    public virtual User? UpdatedByNavigation { get; set; }
}
