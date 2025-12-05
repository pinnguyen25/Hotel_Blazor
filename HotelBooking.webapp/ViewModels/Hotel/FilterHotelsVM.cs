public class FilterHotelsVM
{
    public string? CityIds { get; set; }               // "1,5,10"
    public string? AccommodationTypeIds { get; set; }  // "2012,2014"
    public string? AmenityIds { get; set; }            // "1,3,7"
    public string? BedTypeIds { get; set; }
    public string? ViewTypeIds { get; set; }
    public string? ChainIds { get; set; }
    public string? PolicyIds { get; set; }

    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public decimal? RatingMin { get; set; }           // 4.0, 4.5,...

    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public int? Adults { get; set; }
    public int? Children { get; set; }
    public int? Rooms { get; set; }

    public string SortBy { get; set; } = "Recommended"; // Recommended, PriceAsc, PriceDesc, RatingDesc, Newest
    public int? Skip { get; set; }
    public int? Take { get; set; }
}