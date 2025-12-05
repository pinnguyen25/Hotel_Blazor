// Admin VM
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
public class ChainVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
}

public class ChainCreateOrUpdateVM
{
    [Required] public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public IBrowserFile? LogoFile { get; set; }
    // public string? Website { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
}