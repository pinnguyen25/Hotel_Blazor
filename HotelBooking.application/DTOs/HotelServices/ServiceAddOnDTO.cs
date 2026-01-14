public class ServiceAddOnDTO
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public string? Description { get; set; }
}