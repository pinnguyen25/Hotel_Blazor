public class CityDTO
{
    public int Id { get; set; }
    public int CountryId { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
}