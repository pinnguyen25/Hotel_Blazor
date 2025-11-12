using HotelBooking.infrastructure.Models;
public interface IAccommodationRepository : IRepository<AccommodationType> { }
public class AccommodationRepository : Repository<AccommodationType>, IAccommodationRepository
{
    public AccommodationRepository(HotelBookingContext context) : base(context) { }
}