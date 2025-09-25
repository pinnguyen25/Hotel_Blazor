using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface IHotelService
{
    public Task<string> GetOwnerDashBoard(int ownerId);
    public Task<string> GetHotelByIdAsync(int hotelId, int? userId = null);
    public Task<List<string>> GetHighlyRatedHotelsAsync(int? userId = null);
    public Task<List<SearchHotelResultDTO>> GetSearchOptionsAsync(string cityName, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms);
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

    public async Task<List<SearchHotelResultDTO>> GetSearchOptionsAsync(string cityName, DateTime? checkIn, DateTime? checkOut,
    int? adults, int? children, int? rooms)
    {
        var hotels = await _context.Database
    .SqlQueryRaw<SearchHotelResultDTO>(
        "EXEC sp_SearchHotels @CityName={0}, @CheckIn={1}, @CheckOut={2}, @Adults={3}, @Children={4}, @Rooms={5}",
        cityName, checkIn, checkOut, adults, children, rooms)
    .ToListAsync();
        return hotels;
    }
}