public class PolicyTypeVM
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; } = true;
    public int PolicyCount { get; set; }  
    public List<PolicyVM>? Policies { get; set; } = new List<PolicyVM>();
}