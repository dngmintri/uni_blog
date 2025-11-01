using Blazorise;
namespace frontend.Services;

public interface IUploadService
{
    Task<string> UploadImageAsync(IFileEntry file);
}