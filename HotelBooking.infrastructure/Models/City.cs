using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class City
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public string? Slug { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
