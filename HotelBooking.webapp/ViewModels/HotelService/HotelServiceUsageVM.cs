public class HotelServiceUsageVM
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public bool IsActive { get; set; }
}