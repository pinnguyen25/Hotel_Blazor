using System.Text.Json.Serialization;
using HotelBooking.infrastructure.Models;

public class AccommodationTypeDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // SP map để lấy fullname người create/update
    public string? CreatedByName { get; set; } 
}
