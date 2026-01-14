public class FilterHotelsVM
{
    public string? AccommodationTypeIds { get; set; }  // "2012,2014"
    public string? AmenityIds { get; set; }            // "1,3,7"
    public string? BedTypeIds { get; set; }
    public string? ViewTypeIds { get; set; }
    public string? ChainIds { get; set; }
    public string? PolicyIds { get; set; }
    public string? ServiceIds { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public decimal? RatingMin { get; set; }           // 4.0, 4.5,...
    
    public string SortBy { get; set; } = "Recommended";
}