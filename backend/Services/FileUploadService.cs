namespace Backend.Services;

public class FileUploadService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(IWebHostEnvironment env, ILogger<FileUploadService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<string?> SaveImageAsync(IFormFile file, string subfolder)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
                return null;

            // Validate extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type. Only jpg, jpeg, png, gif, webp are allowed.");

            // Validate size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("File size exceeds 5MB limit.");

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            
            // Create upload path
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(uploadPath);

            // Save file
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL
            return $"/uploads/{subfolder}/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving image");
            throw;
        }
    }

    public bool DeleteImage(string? imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
            return false;
        }
    }
}