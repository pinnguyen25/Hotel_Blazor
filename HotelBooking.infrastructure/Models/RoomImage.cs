using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class RoomImage
{
    public int Id { get; set; }

    public int RoomTypeId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Additional { get; set; }

    public bool? IsDefault { get; set; }

    public int? SortOrder { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual RoomType RoomType { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
