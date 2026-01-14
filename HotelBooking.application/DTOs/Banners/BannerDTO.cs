public class BannerDTO
{
    public int Id { get; set; }
    public string Page { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
