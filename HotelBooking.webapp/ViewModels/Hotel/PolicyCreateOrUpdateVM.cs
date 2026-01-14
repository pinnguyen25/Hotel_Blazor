public class PolicyCreateOrUpdateVM
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int PolicyTypeId { get; set; }
}

public class OwnerCustomPolicyVM
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PolicyTypeId { get; set; }
    public string? PolicyTypeName { get; set; }
}