using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Additional { get; set; }

    public string? Address { get; set; }

    public string? TaxCode { get; set; }

    public virtual ICollection<AccommodationType> AccommodationTypeCreatedByNavigations { get; set; } = new List<AccommodationType>();

    public virtual ICollection<AccommodationType> AccommodationTypeUpdatedByNavigations { get; set; } = new List<AccommodationType>();

    public virtual ICollection<Amenity> AmenityCreatedByNavigations { get; set; } = new List<Amenity>();

    public virtual ICollection<Amenity> AmenityUpdatedByNavigations { get; set; } = new List<Amenity>();

    public virtual ICollection<Booking> BookingCreatedByNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingCustomers { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingUpdatedByNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Chain> ChainCreatedByNavigations { get; set; } = new List<Chain>();

    public virtual ICollection<Chain> ChainUpdatedByNavigations { get; set; } = new List<Chain>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<EventBooking> EventBookingCreatedByNavigations { get; set; } = new List<EventBooking>();

    public virtual ICollection<EventBooking> EventBookingCustomers { get; set; } = new List<EventBooking>();

    public virtual ICollection<EventBooking> EventBookingUpdatedByNavigations { get; set; } = new List<EventBooking>();

    public virtual ICollection<EventType> EventTypeCreatedByNavigations { get; set; } = new List<EventType>();

    public virtual ICollection<EventType> EventTypeUpdatedByNavigations { get; set; } = new List<EventType>();

    public virtual ICollection<Hotel> HotelCreatedByNavigations { get; set; } = new List<Hotel>();

    public virtual ICollection<HotelImage> HotelImageCreatedByNavigations { get; set; } = new List<HotelImage>();

    public virtual ICollection<HotelImage> HotelImageUpdatedByNavigations { get; set; } = new List<HotelImage>();

    public virtual ICollection<Hotel> HotelOwners { get; set; } = new List<Hotel>();

    public virtual ICollection<Hotel> HotelUpdatedByNavigations { get; set; } = new List<Hotel>();

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<User> InverseUpdatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Notification> NotificationCreatedByNavigations { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> PaymentCreatedByNavigations { get; set; } = new List<Payment>();

    public virtual ICollection<Payment> PaymentUpdatedByNavigations { get; set; } = new List<Payment>();

    public virtual ICollection<Policy> PolicyCreatedByNavigations { get; set; } = new List<Policy>();

    public virtual ICollection<Policy> PolicyUpdatedByNavigations { get; set; } = new List<Policy>();

    public virtual ICollection<Review> ReviewCreatedByNavigations { get; set; } = new List<Review>();

    public virtual ICollection<Review> ReviewCustomers { get; set; } = new List<Review>();

    public virtual ICollection<Review> ReviewUpdatedByNavigations { get; set; } = new List<Review>();

    public virtual ICollection<Room> RoomCreatedByNavigations { get; set; } = new List<Room>();

    public virtual ICollection<RoomImage> RoomImageCreatedByNavigations { get; set; } = new List<RoomImage>();

    public virtual ICollection<RoomImage> RoomImageUpdatedByNavigations { get; set; } = new List<RoomImage>();

    public virtual ICollection<RoomType> RoomTypeCreatedByNavigations { get; set; } = new List<RoomType>();

    public virtual ICollection<RoomType> RoomTypeUpdatedByNavigations { get; set; } = new List<RoomType>();

    public virtual ICollection<Room> RoomUpdatedByNavigations { get; set; } = new List<Room>();

    public virtual ICollection<Service> ServiceCreatedByNavigations { get; set; } = new List<Service>();

    public virtual ICollection<Service> ServiceUpdatedByNavigations { get; set; } = new List<Service>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UpgradeRequest> UpgradeRequestApprovedByNavigations { get; set; } = new List<UpgradeRequest>();

    public virtual ICollection<UpgradeRequest> UpgradeRequestCreatedByNavigations { get; set; } = new List<UpgradeRequest>();

    public virtual ICollection<UpgradeRequest> UpgradeRequestUpdatedByNavigations { get; set; } = new List<UpgradeRequest>();

    public virtual ICollection<UpgradeRequest> UpgradeRequestUsers { get; set; } = new List<UpgradeRequest>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
