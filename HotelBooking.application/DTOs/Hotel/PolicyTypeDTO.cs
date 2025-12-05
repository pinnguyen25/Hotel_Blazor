using System.Text.Json.Serialization;

public class PolicyTypeDTO
{
    public int Id { get; set; }
    public string? Code { get; set; } // e.g., "CANCELLATION", "CHECKIN"
    public string? Name { get; set; } // e.g., "Chính sách nhận phòng", "Chính sách hủy phòng"
    public bool IsActive { get; set; } = true;
    public int PolicyCount { get; set; }  
    public List<PolicyDTO>? Policies { get; set; } = new List<PolicyDTO>();
    [JsonIgnore]
    public string? PoliciesJson { get; set; }
}