public class ServicesVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? CreatedBy { get; set; }
    public int? CreatedAt { get; set; }
    public int? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
    public bool? IsDeleted {get;set;}
    public bool IsSelected { get; set; }
}

