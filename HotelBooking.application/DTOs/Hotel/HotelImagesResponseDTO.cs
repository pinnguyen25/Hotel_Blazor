public class HotelImagesResponseDTO
{
    public string? CoverImageUrl { get; set; }
    public List<string> GalleryImageUrls { get; set; } = new();
}