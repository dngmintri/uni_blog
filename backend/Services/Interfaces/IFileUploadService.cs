namespace Backend.Services.Interfaces;

public interface IFileUploadService
{
    Task<string?> UploadFileAsync(IFormFile file, string subfolder);
    bool DeleteFile(string? fileUrl);
}

