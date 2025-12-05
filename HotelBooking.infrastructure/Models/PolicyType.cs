using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class PolicyType
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public string? Icon { get; set; }

    public bool IsActive { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int PolicyCount { get; set; }

    public int? CreatedBy { get; set; }

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
