using System.Text.Json;
using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface IWishlistService
{
    public Task<bool> ToggleWishlistAsync(int userId, int hotelId);
    public Task<bool> IsInWishlistAsync(int userId, int hotelId);
    public Task<List<HotelListItemDTO>> GetUserWishlistAsync(int userId); // lấy danh sách hotel đã lưu
}

public class WishlistService : IWishlistService
{
    private readonly HotelBookingContext _context;
    public WishlistService(HotelBookingContext context)
    {
        _context = context;
    }

    public async Task<bool> ToggleWishlistAsync(int userId, int hotelId)
    {
        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.HotelId == hotelId);
        if (wishlist != null)
        {
            _context.Wishlists.Remove(wishlist);
        }
        else
        {
            _context.Wishlists.Add(new Wishlist { UserId = userId, HotelId = hotelId, CreatedAt = DateTime.Now });
        }

        await _context.SaveChangesAsync();
        return wishlist == null; // True nếu thêm wishlist mới , False nếu xóa
    }

    public async Task<bool> IsInWishlistAsync(int userId, int hotelId)
    {
        return await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.HotelId == hotelId);
    }

    public async Task<List<HotelListItemDTO>> GetUserWishlistAsync(int userId)
    {
        // Query tối ưu, lọc bỏ khách sạn đã xóa/tạm ngưng
        var wishlists = await _context.Wishlists
            .AsNoTracking()
            .Include(w => w.Hotel) // Include sơ bộ để check điều kiện
            .Where(w => w.UserId == userId
                     && w.Hotel.IsDeleted == false
                     && w.Hotel.Status == "Active") // Chỉ lấy khách sạn đang hoạt động
            .OrderByDescending(w => w.CreatedAt) // Mới lưu lên đầu
            .Select(w => new
            {
                // Select Anonymous Type trước để tối ưu query SQL
                Hotel = w.Hotel,
                // Lấy ảnh đã sort
                Images = w.Hotel.HotelImages
                            .Where(i => i.IsDeleted == false)
                            .OrderBy(i => i.SortOrder)
                            .Select(i => i.ImageUrl)
                            .Take(5)
                            .ToList(),
                // Lấy amenities xịn nhất
                RawAmenities = w.Hotel.HotelAmenities
                            .Where(ha => ha.Amenity.IsDeleted == false)
                            .OrderByDescending(ha => ha.Amenity.IsFilterable)
                            .Select(ha => new
                            {
                                ha.Amenity.Id,
                                ha.Amenity.Name,
                                ha.Amenity.Additional // <--- CẦN CÁI NÀY
                            })
                            .Take(3)
                            .ToList(),
                // Tính toán giá và review
                MinPrice = w.Hotel.RoomTypes.Where(rt => rt.IsDeleted == false).Min(rt => (decimal?)rt.PricePerNight),
                MaxPrice = w.Hotel.RoomTypes.Where(rt => rt.IsDeleted == false).Max(rt => (decimal?)rt.PricePerNight),
                AvgRating = w.Hotel.Reviews.Where(r => r.IsDeleted == false).Average(r => (decimal?)r.Rating) ?? 0,
                ReviewCount = w.Hotel.Reviews.Count(r => r.IsDeleted == false),
                AvailableRooms = w.Hotel.RoomTypes.SelectMany(rt => rt.Rooms)
                                .Count(r => r.Status == "Available" && r.IsDeleted == false)
            })
            .ToListAsync();

        // Map sang DTO
        var result = wishlists.Select(item => new HotelListItemDTO
        {
            HotelId = item.Hotel.Id,
            HotelName = item.Hotel.Name,
            Address = item.Hotel.Address,
            City = item.Hotel.City?.Name ?? "", // Cần Include City ở trên hoặc chấp nhận null nếu Lazy Loading tắt
                                                // Country = ..., 

            // Logic ảnh giống HotelService: Ưu tiên Cover, nếu không có thì lấy ảnh đầu tiên trong Gallery
            CoverImageUrl = !string.IsNullOrEmpty(item.Hotel.CoverImageUrl)
                            ? item.Hotel.CoverImageUrl
                            : item.Images.FirstOrDefault() ?? "/images/default-hotel.jpg",

            ImageUrls = item.Images,
            HighlightAmenities = item.RawAmenities.Select(a =>
                {
                    var iconInfo = TryParseAmenityAdditional(a.Additional);
                    return new AmenityDTO
                    {
                        Id = a.Id,
                        Name = a.Name,
                        // Gán giá trị mặc định nếu không có trong JSON
                        IconClass = iconInfo.ContainsKey("IconClass") ? iconInfo["IconClass"] : "bi bi-check-circle",
                        IconColor = iconInfo.ContainsKey("IconColor") ? iconInfo["IconColor"] : "#54a9ff"
                    };
                }).ToList(),

            MinPricePerNight = item.MinPrice,
            MaxPricePerNight = item.MaxPrice,

            AverageRating = Math.Round(item.AvgRating, 1),
            ReviewCount = item.ReviewCount,

            AvailableRooms = item.AvailableRooms,
            IsBookable = item.AvailableRooms > 0, // Đơn giản hóa

            IsWishlist = true, // Chắc chắn là true vì đang ở trang wishlist

            // ShortDescription xử lý cắt chuỗi an toàn
            ShortDescription = !string.IsNullOrEmpty(item.Hotel.Description)
                ? (item.Hotel.Description.Length > 150 ? item.Hotel.Description.Substring(0, 150) + "..." : item.Hotel.Description)
                : string.Empty
        }).ToList();

        return result;
    }
    private Dictionary<string, string> TryParseAmenityAdditional(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new Dictionary<string, string>();
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}