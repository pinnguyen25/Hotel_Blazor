using System.ComponentModel.DataAnnotations.Schema;

public class SearchHotelResultDTO
{
    public int HotelId { get; set; }
    public string HotelName  { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,4)")]
    public decimal? MinPrice { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal? MaxPrice { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal? AvgPrice { get; set; }
    public int AvailableRooms { get; set; }
    public decimal AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public string? Images { get; set; } = "[]";         // JSON string
    public string Amenities { get; set; } = string.Empty;   // JSON string
    public string AccommodationType { get; set; } = string.Empty;  // ← từ act.Name
    public string ChainName { get; set; } = string.Empty;
    public string Services {get;set;} = string.Empty;
    // public int TotalCount { get; set; } = 0;
}