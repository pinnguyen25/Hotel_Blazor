public class ServicesDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? CreatedBy { get; set; }
    public int? UpdateBy { get; set; }
    public bool? IsDeleted {get;set;}
}

public class ServiceCreateOrUpdateDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}