using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class HotelImage
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Additional { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
