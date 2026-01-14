using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class RoomTypeService
{
    public int Id { get; set; }

    public int RoomTypeId { get; set; }

    public int ServiceId { get; set; }

    public int? Quantity { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual RoomType RoomType { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
