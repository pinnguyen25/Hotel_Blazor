public class HotelDraftFullDTO
{
    public HotelResponseDTO BasicInfo { get; set; } = new();
    public HotelImagesResponseDTO Images { get; set; } = new();
    public List<int> SelectedAmenityIds { get; set; } = new();
    public List<int> SelectedPolicyIds { get; set; } = new();
    public List<OwnerCustomPolicyDTO> CustomPolicies { get; set; } = new();
}