public class PolicyCreateOrUpdateDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int PolicyTypeId { get; set; }
}

public class OwnerCustomPolicyDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PolicyTypeId { get; set; }
}