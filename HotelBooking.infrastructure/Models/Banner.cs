using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Banner
{
    public int Id { get; set; }

    public string Page { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string? PublicId { get; set; }

    public string? LinkUrl { get; set; }

    public string? Title { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
