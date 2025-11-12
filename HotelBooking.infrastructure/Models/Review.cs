using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Review
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public int CustomerId { get; set; }

    public decimal? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Additional { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
