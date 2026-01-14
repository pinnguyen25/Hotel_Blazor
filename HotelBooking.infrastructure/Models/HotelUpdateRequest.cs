using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class HotelUpdateRequest
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public int OwnerId { get; set; }

    public string UpdateContentJson { get; set; } = null!;

    public string? Status { get; set; }

    public string? AdminNote { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;

    public virtual User? ProcessedByNavigation { get; set; }
}
