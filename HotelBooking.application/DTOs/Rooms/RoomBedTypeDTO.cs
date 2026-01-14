public class RoomBedTypeDTO
{
    public int RoomTypeId { get; set; }
    public int BedTypeId { get; set; }
    public string? BedTypeName {get;set;}
    public string? Additional {get;set;}
    public int Quantity { get; set; } = 1;
    public bool? IsPrimary { get; set; } = false;
    
}