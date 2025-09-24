using HotelBooking.infrastructure.Models;

public interface IHotelService
{
    public Task<string> GetOwnerDashBoard(int ownerId);
    public Task<string> GetHotelByIdAsync(int hotelId, int? userId = null);
    public Task<List<string>> GetHighlyRatedHotelsAsync(int? userId = null);
}

public class HotelService : IHotelService
{
    private readonly HotelBookingContext _context; // check wishlist
    private readonly IHotelRepository _hotelRepository;
    public IUnitOfWork _dbu;

    public HotelService(HotelBookingContext context, IHotelRepository hotelRepository, IUnitOfWork dbu)
    {
        _context = context;
        _hotelRepository = hotelRepository;
        _dbu = dbu;
    }

    public Task<List<string>> GetHighlyRatedHotelsAsync(int? userId = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetHotelByIdAsync(int hotelId, int? userId = null)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetOwnerDashBoard(int ownerId)
    {
        return await Task.FromResult($"Owner Dashboard for Owner ID: {ownerId}");
    }

}