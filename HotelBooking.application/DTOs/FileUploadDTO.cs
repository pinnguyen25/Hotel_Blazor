using System.ComponentModel.DataAnnotations;

public class FileUploadDTO
{
    [Required(ErrorMessage = "Image file is required")]
    public IFormFile? Image { get; set; }
}