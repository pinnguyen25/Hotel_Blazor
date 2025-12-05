using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Staff
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int HotelId { get; set; }

    public string? Position { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Additional { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
