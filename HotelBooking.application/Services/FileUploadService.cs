using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public interface IFileUploadService
{
    public Task<string?> SaveImageAsync(IFormFile img, string folderName, string? publicIdHint = null);
    public Task DeleteImageAsync(string? imageUrl);
    Task DeleteFolderAsync(string folderName);
}

public class FileUploadService : IFileUploadService
{
    private readonly Cloudinary _cloudinary;
    public FileUploadService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string?> SaveImageAsync(IFormFile imageFile, string folderName, string? publicIdHint = null)
    {
        if (imageFile == null || imageFile.Length == 0) return null;

        await using var stream = imageFile.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(imageFile.FileName, stream),
            Folder = folderName, // Tự động tạo folder trên Cloudinary
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = true,
            PublicId = publicIdHint
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        // Trả về URL an toàn (https) của ảnh
        return uploadResult.Error == null
            ? uploadResult.SecureUrl.ToString()
            : null;
    }

    public async Task DeleteImageAsync(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }
        var publicId = ExtractPublicIdFromUrl(imageUrl);
        if (string.IsNullOrEmpty(publicId))
            return;

        try
        {
            var deletionResult = await _cloudinary.DestroyAsync(new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            });

            // Optional: log if result.Result != "ok"
            if (deletionResult.Result != "ok")
            {
                Console.WriteLine($"Cloudinary delete failed: {deletionResult.Error?.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteImage error: {ex.Message}");
            // Không throw để không làm crash API
        }

    }

    public async Task DeleteFolderAsync(string folderName)
    {
        try
        {
            var result = await _cloudinary.DeleteResourcesAsync( new DelResParams
            {
                ResourceType = ResourceType.Image,
                All = true,
                Prefix = folderName
            });

            Console.WriteLine($"Deleted {result?.Deleted?.Count ?? 0} images in folder: {folderName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteFolder error: {ex.Message}");
        }
    }

    private static string? ExtractPublicIdFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath;

            // Tìm vị trí sau /upload/
            // /v1234567890/hotels/1/2/cover.jpg → hotels/1/2/cover
            int start = path.IndexOf("upload/", StringComparison.Ordinal) + 8;
            if (start <= 7) return null;

            var publicId = path.Substring(start);
            publicId = publicId.Split('?')[0]; // bỏ query string
            return Path.ChangeExtension(publicId.Trim('/'), null); // bỏ .jpg
        }
        catch
        {
            return null;
        }
    }
}