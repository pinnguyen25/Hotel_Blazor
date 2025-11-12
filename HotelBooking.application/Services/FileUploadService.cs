using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public interface IFileUploadService
{
    public Task<string?> SaveImageAsync(IFormFile img, string folderName);
    public Task DeleteImage(string? imagePath);
}

public class FileUploadService : IFileUploadService
{
    private readonly Cloudinary _cloudinary;
    public FileUploadService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string?> SaveImageAsync(IFormFile imageFile, string folderName)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null; // Hoặc string.Empty
        }

        await using var stream = imageFile.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(imageFile.FileName, stream),
            Folder = folderName, // Tự động tạo folder trên Cloudinary
            // Thêm logic biến đổi ảnh, ví dụ: resize
            // Transformation = new Transformation().Width(800).Height(600).Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            // Xử lý lỗi (ví dụ: log lại)
            return null;
        }

        // Trả về URL an toàn (https) của ảnh
        return uploadResult.SecureUrl.ToString();
    }

    public async Task DeleteImage(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }
        
        try
        {
            Uri uri = new Uri(imagePath);
            // Lấy phần path, loại bỏ / và phần mở rộng file
            string path = uri.AbsolutePath;
            // Bắt đầu từ tên folder
            var segments = uri.Segments;
            string? folderName = null;
            for(int i = 0; i < segments.Length; i++)
            {
                if(segments[i].Contains("upload") && i + 2 < segments.Length)
                {
                    folderName = segments[i+1].Replace("/", ""); // Ví dụ: lấy "accommodation"
                    break;
                }
            }

            if(folderName == null) return; // Không tìm thấy folder hợp lệ

            int startIndex = path.IndexOf(folderName);
            if (startIndex == -1) return; // Không tìm thấy folder

            string publicIdWithExtension = path.Substring(startIndex);
            string publicId = Path.ChangeExtension(publicIdWithExtension, null);

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            // Gọi API xóa của Cloudinary
            await _cloudinary.DestroyAsync(deletionParams);
        }
        catch (Exception)
        {
            // Xử lý lỗi (ví dụ: log lại, URL không hợp lệ)
        }

    }
}