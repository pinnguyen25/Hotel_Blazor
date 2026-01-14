using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class RoomType
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PricePerNight { get; set; }

    public int? AdultCapacity { get; set; }

    public int? ChildCapacity { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Additional { get; set; }

    public bool? IsActive { get; set; }

    public string? DefaultImageUrl { get; set; }

    public int? SortOrder { get; set; }

    public decimal? WeekendPrice { get; set; }

    public int? MinStayNights { get; set; }

    public int? MaxStayNights { get; set; }

    public int? MaxGuests { get; set; }

    public int Quantity { get; set; }

    public decimal? Area { get; set; }

    public bool IsFreeCancellation { get; set; }

    public bool IsBreakfastIncluded { get; set; }

    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();

    public virtual ICollection<RoomBedType> RoomBedTypes { get; set; } = new List<RoomBedType>();

    public virtual ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();

    public virtual ICollection<RoomTypeService> RoomTypeServices { get; set; } = new List<RoomTypeService>();

    public virtual ICollection<RoomViewType> RoomViewTypes { get; set; } = new List<RoomViewType>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual User? UpdatedByNavigation { get; set; }
}
