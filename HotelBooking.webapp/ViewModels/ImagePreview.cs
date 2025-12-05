using Microsoft.AspNetCore.Components.Forms;

public class ImagePreview
{
    public IBrowserFile? File { get; set; }
    public string? Url { get; set; }
    public bool IsExisting { get; set; }
}