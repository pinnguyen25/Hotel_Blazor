using HotelBooking.infrastructure.Models;

public interface IBannerRepository : IRepository<Banner> { }
public class BannerRepository : Repository<Banner>, IBannerRepository
{
    public BannerRepository(HotelBookingContext context) : base(context) { }
}
