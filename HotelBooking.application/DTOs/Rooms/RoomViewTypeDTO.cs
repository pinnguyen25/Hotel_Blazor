public class RoomViewTypeDTO
{
    public int RoomTypeId { get; set; }
    public int ViewTypeId { get; set; }
    public string? ViewTypeName {get;set;}
    public string? Additional {get;set;}
    public bool IsPrimary { get; set; } = false;
    
}