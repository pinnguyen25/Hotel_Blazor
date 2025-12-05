using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class BedType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconClass { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Additional { get; set; }

    public virtual ICollection<RoomBedType> RoomBedTypes { get; set; } = new List<RoomBedType>();
}
