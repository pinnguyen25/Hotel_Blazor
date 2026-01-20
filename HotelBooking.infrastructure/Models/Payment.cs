using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string TransactionId { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Additional { get; set; }

    public string? Type { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
