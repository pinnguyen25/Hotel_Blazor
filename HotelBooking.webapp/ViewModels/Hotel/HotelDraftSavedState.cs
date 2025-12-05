namespace HotelBooking.Client.Models;

[Serializable]
public class HotelDraftSavedState
{
    public const string STORAGE_KEY = "hotel_draft_state_v1";

    public int DraftHotelId { get; set; }
    public HotelCreateOrUpdateVM? BasicInfo { get; set; }
    public HotelImagesVM? Images { get; set; }
    public List<int> SelectedAmenityIds { get; set; } = new();
    public Dictionary<int, int> SelectedPolicyByType { get; set; } = new();
    public List<string> CustomPolicies { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}