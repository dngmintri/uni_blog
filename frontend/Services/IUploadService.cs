using Microsoft.AspNetCore.Components.Forms;
using Blazorise;

public interface IUploadService
{
    Task<string> UploadImageAsync(IFileEntry file);
}