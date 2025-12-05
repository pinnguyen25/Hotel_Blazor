public class PolicyVM
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsDeleted { get; set; }
    public int PolicyTypeId { get; set; }
    public bool? IsSystemPolicy { get; set; } = true;
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public PolicyTypeVM? PolicyType { get; set; }
}