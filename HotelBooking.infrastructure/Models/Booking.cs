using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int HotelId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Additional { get; set; }

    public string ContactName { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public string? Note { get; set; }

    public decimal? DiscountAmount { get; set; }

    public int? PromotionId { get; set; }

    public decimal? OriginalPrice { get; set; }

    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Promotion? Promotion { get; set; }

    public virtual Review? Review { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
