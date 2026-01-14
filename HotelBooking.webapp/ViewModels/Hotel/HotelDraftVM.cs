public class HotelDraftVM
{
    public HotelCreateOrUpdateVM BasicInfo { get; set; } = new();
    public HotelImagesVM Images { get; set; } = new();
    public List<int> SelectedAmenityIds { get; set; } = new();
    public List<int> SelectedPolicyIds { get; set; } = new();
    public List<OwnerCustomPolicyVM> CustomPolicies { get; set; } = new();
    public List<OwnerHotelServiceVM>? HotelServices { get; set; } = new();
    public List<RoomTypeDetailVM> RoomTypes { get; set; } = new();
    public List<PolicyVM> SystemPolicies { get; set; } = new();
}