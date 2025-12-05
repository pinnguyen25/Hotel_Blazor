using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Chain
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    public string? Website { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Additional { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    public virtual User? UpdatedByNavigation { get; set; }
}
