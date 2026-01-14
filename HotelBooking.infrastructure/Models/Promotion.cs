using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Promotion
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public int? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public decimal? MinBookingValue { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }

    public int? UserUsageLimit { get; set; }

    public bool? IsSystemWide { get; set; }

    public int? HotelId { get; set; }

    public int? PromotionCategory { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
