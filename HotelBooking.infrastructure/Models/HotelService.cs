using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class HotelService
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public int ServiceId { get; set; }

    public decimal Price { get; set; }

    public string? Unit { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
