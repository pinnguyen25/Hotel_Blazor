using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.infrastructure.Models;

public partial class HotelBookingContext : DbContext
{
    public HotelBookingContext()
    {
    }

    public HotelBookingContext(DbContextOptions<HotelBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccommodationEvent> AccommodationEvents { get; set; }

    public virtual DbSet<AccommodationType> AccommodationTypes { get; set; }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingRoom> BookingRooms { get; set; }

    public virtual DbSet<BookingService> BookingServices { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EventBooking> EventBookings { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelAmenity> HotelAmenities { get; set; }

    public virtual DbSet<HotelImage> HotelImages { get; set; }

    public virtual DbSet<HotelPolicy> HotelPolicies { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomAmenity> RoomAmenities { get; set; }

    public virtual DbSet<RoomImage> RoomImages { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<UpgradeRequest> UpgradeRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=connectionStringHotelBooking");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccommodationEvent>(entity =>
        {
            entity.HasKey(e => new { e.AccommodationTypeId, e.EventTypeId }).HasName("PK__Accommod__B7275FD691FFF1C7");

            entity.ToTable("AccommodationEvent");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AccommodationType).WithMany(p => p.AccommodationEvents)
                .HasForeignKey(d => d.AccommodationTypeId)
                .HasConstraintName("FK_AccommodationEvent_AccommodationTypes");

            entity.HasOne(d => d.EventType).WithMany(p => p.AccommodationEvents)
                .HasForeignKey(d => d.EventTypeId)
                .HasConstraintName("FK_AccommodationEvent_EventTypes");
        });

        modelBuilder.Entity<AccommodationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accommod__3214EC07778062D1");

            entity.HasIndex(e => e.CreatedBy, "IX_AccommodationTypes_CreatedBy");

            entity.HasIndex(e => e.IsDeleted, "IX_AccommodationTypes_IsDeleted");

            entity.HasIndex(e => e.Slug, "UQ_AccommodationTypes_Slug").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Accommod__BC7B5FB6F1B50823").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AccommodationTypeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_AccommodationTypes_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AccommodationTypeUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_AccommodationTypes_UpdatedBy");
        });

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Amenitie__3214EC07A561F2C6");

            entity.HasIndex(e => e.Name, "UQ__Amenitie__737584F6647CACE1").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IconClass).HasMaxLength(50);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AmenityCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Amenities_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AmenityUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Amenities_UpdatedBy");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC0710412DC0");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("PendingPayment");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BookingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Bookings_CreatedBy");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Customers");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Hotels");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_RoomTypes");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BookingUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Bookings_UpdatedBy");
        });

        modelBuilder.Entity<BookingRoom>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.RoomId }).HasName("PK__BookingR__F0BD797EB3B25463");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingRooms)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingRooms_Bookings");

            entity.HasOne(d => d.Room).WithMany(p => p.BookingRooms)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingRooms_Rooms");
        });

        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingS__3214EC071AD6AD2E");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPaid).HasDefaultValue(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingServices_Bookings");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_BookingServices_Services");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cities__3214EC07B7CC9780");

            entity.ToTable(tb => tb.HasTrigger("trg_Cities_UpdateNameUnaccented"));

            entity.HasIndex(e => new { e.Name, e.CountryId }, "UQ_Cities_Name_Country").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NameUnaccented).HasMaxLength(200);

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_Cities_Countries");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07D86E51C6");

            entity.HasIndex(e => e.Name, "UQ__Countrie__737584F6D94F2AA5").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<EventBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventBoo__3214EC072CB1F282");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EventBookingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_EventBookings_CreatedBy");

            entity.HasOne(d => d.Customer).WithMany(p => p.EventBookingCustomers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_EventBookings_Users");

            entity.HasOne(d => d.EventType).WithMany(p => p.EventBookings)
                .HasForeignKey(d => d.EventTypeId)
                .HasConstraintName("FK_EventBookings_EventTypes");

            entity.HasOne(d => d.Hotel).WithMany(p => p.EventBookings)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_EventBookings_Hotels");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EventBookingUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_EventBookings_UpdatedBy");
        });

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventTyp__3214EC07D2BB59C4");

            entity.HasIndex(e => e.Name, "UQ__EventTyp__737584F662E826A5").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__EventTyp__BC7B5FB6F704E934").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(100);
            entity.Property(e => e.SuitableFor).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EventTypeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_EventTypes_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EventTypeUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_EventTypes_UpdatedBy");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hotels__3214EC07A489B01A");

            entity.ToTable(tb => tb.HasTrigger("trg_Hotels_UpdateNameUnaccented"));

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.NameUnaccented).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("PendingVerification");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.AccommodationType).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.AccommodationTypeId)
                .HasConstraintName("FK_Hotels_AccommodationTypes");

            entity.HasOne(d => d.City).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Hotels_Cities");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.HotelCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Hotels_CreatedBy");

            entity.HasOne(d => d.Owner).WithMany(p => p.HotelOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hotels_Owner");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.HotelUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Hotels_UpdatedBy");
        });

        modelBuilder.Entity<HotelAmenity>(entity =>
        {
            entity.HasKey(e => new { e.HotelId, e.AmenityId }).HasName("PK__HotelAme__EE40948FD2287634");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Amenity).WithMany(p => p.HotelAmenities)
                .HasForeignKey(d => d.AmenityId)
                .HasConstraintName("FK_HotelAmenities_Amenities");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelAmenities)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_HotelAmenities_Hotels");
        });

        modelBuilder.Entity<HotelImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HotelIma__3214EC0781C36DA6");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.HotelImageCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_HotelImages_CreatedBy");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelImages)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_HotelImages_Hotels");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.HotelImageUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_HotelImages_UpdatedBy");
        });

        modelBuilder.Entity<HotelPolicy>(entity =>
        {
            entity.HasKey(e => new { e.HotelId, e.PolicyId }).HasName("PK__HotelPol__14E30845ED5B0213");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelPolicies)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_HotelPolicies_Hotels");

            entity.HasOne(d => d.Policy).WithMany(p => p.HotelPolicies)
                .HasForeignKey(d => d.PolicyId)
                .HasConstraintName("FK_HotelPolicies_Policies");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC07AE8CC08F");

            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Booking).WithMany(p => p.Messages)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Messages_Bookings");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Messages)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_Messages_HotelId");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Sender");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07239AA8F4");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(255);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.NotificationCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Notifications_CreatedBy");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07F143B5C6");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaidAt).HasPrecision(0);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransactionId).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Payments_Bookings");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PaymentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Payments_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PaymentUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Payments_UpdatedBy");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Policies__3214EC070811526F");

            entity.HasIndex(e => e.Name, "UQ__Policies__737584F6462C829E").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PolicyCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Policies_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PolicyUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Policies_UpdatedBy");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC0718171E72");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ReviewCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Reviews_CreatedBy");

            entity.HasOne(d => d.Customer).WithMany(p => p.ReviewCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Customers");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_Reviews_Hotels");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ReviewUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Reviews_UpdatedBy");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07652222B1");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F69B2FDE49").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3214EC076CED4B63");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.RoomNumber).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoomCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Rooms_CreatedBy");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_Rooms_RoomTypes");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoomUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Rooms_UpdatedBy");
        });

        modelBuilder.Entity<RoomAmenity>(entity =>
        {
            entity.HasKey(e => new { e.RoomTypeId, e.AmenityId }).HasName("PK__RoomAmen__148A3961B65F81DB");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Amenity).WithMany(p => p.RoomAmenities)
                .HasForeignKey(d => d.AmenityId)
                .HasConstraintName("FK_RoomAmenities_Amenities");

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomAmenities)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_RoomAmenities_RoomTypes");
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomImag__3214EC073F95FA12");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoomImageCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_RoomImages_CreatedBy");

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomImages)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK_RoomImages_RoomTypes");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoomImageUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_RoomImages_UpdatedBy");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomType__3214EC0773B0611B");

            entity.HasIndex(e => new { e.HotelId, e.Name }, "UQ_RoomTypes_Name_Hotel").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoomTypeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_RoomTypes_CreatedBy");

            entity.HasOne(d => d.Hotel).WithMany(p => p.RoomTypes)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_RoomTypes_Hotels");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoomTypeUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_RoomTypes_UpdatedBy");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Services__3214EC07475034AA");

            entity.HasIndex(e => e.Name, "UQ__Services__737584F699954E73").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ServiceCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Services_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ServiceUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Services_UpdatedBy");
        });

        modelBuilder.Entity<UpgradeRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UpgradeR__3214EC072C43C772");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RequestedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TaxCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.UpgradeRequestApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_UpgradeRequests_ApprovedBy");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UpgradeRequestCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_UpgradeRequests_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UpgradeRequestUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_UpgradeRequests_UpdatedBy");

            entity.HasOne(d => d.User).WithMany(p => p.UpgradeRequestUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UpgradeRequests_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079F4E555A");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B1FEF9B1").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F28456B9896957").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.TaxCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Users_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Users_UpdatedBy");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF2760AD6965D04F");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.HotelId }).HasName("PK__Wishlist__F3E8EFF1EF67BFFA");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_Wishlists_Hotels");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Wishlists_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
