public class HotelAdminDTO : HotelBaseDTO
{
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public int? CreatedBy { get; set; }

    public OwnerDashboardStatsDTO Stats { get; set; } = new();
}