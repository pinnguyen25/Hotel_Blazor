using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class Hotel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Description { get; set; }

    public string? CoverImageUrl { get; set; }

    public int OwnerId { get; set; }

    public int CityId { get; set; }

    public int? AccommodationTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsVerified { get; set; }

    public bool? IsDeleted { get; set; }

    public string? NameUnaccented { get; set; }

    public string? Additional { get; set; }

    public int? ChainId { get; set; }

    public string? Status { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public bool IsActive { get; set; }

    public virtual AccommodationType? AccommodationType { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Chain? Chain { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<EventBooking> EventBookings { get; set; } = new List<EventBooking>();

    public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();

    public virtual ICollection<HotelImage> HotelImages { get; set; } = new List<HotelImage>();

    public virtual ICollection<HotelPolicy> HotelPolicies { get; set; } = new List<HotelPolicy>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
