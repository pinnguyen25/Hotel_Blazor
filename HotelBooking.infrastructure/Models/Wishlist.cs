using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Wishlist
{
    public int UserId { get; set; }

    public int HotelId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
