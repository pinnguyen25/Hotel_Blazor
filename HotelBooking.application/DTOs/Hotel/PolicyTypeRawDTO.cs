internal class PolicyTypeRawDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PolicyTypeId { get; set; }
    public bool IsSystemPolicy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Từ PolicyType
    public string? PolicyTypeCode { get; set; }
    public string? PolicyTypeName { get; set; }
    public bool PolicyTypeIsActive { get; set; }
}