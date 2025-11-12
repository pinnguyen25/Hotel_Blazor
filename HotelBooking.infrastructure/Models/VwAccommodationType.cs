using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class VwAccommodationType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Slug { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public string? CreatedByName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
