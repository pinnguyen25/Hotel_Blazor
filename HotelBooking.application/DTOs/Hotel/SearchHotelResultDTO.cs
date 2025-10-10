public class SearchHotelResultDTO
{
    public int HotelId { get; set; }
    public string HotelName  { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? AvgPrice { get; set; }
    public int AvailableRooms { get; set; }
    public double AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public string? Images { get; set; } = "[]";         // JSON string
    public string AmenityNames { get; set; } = string.Empty;   // JSON string
}