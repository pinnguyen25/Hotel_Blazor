public class BannerVM
{
    public int Id { get; set; }
    public string? Page { get; set; }
    public string? ImageUrl { get; set; }
    public string? Title { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; } 
    public bool IsActive { get; set; } = true;
}

public class BannerUpdateVM
{
    public string? Page { get; set; }
    public string? Title { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}