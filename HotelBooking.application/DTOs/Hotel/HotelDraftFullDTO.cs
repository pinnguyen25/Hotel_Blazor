public class HotelDraftFullDTO
{
    public HotelResponseDTO BasicInfo { get; set; } = new();
    public HotelImagesResponseDTO Images { get; set; } = new();
    public List<int> SelectedAmenityIds { get; set; } = new();
    public List<int> SelectedPolicyIds { get; set; } = new();
    public List<OwnerCustomPolicyDTO> CustomPolicies { get; set; } = new();
    public List<OwnerHotelServiceDTO>? HotelServices { get; set; }
    public List<RoomTypeForOwnerDTO> RoomTypes { get; set; } = new();
    public List<PolicyDTO> SystemPolicies { get; set; } = new();
    public string? RejectionReason { get; set; }
}