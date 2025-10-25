using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazorise;
using Microsoft.JSInterop;
public class UploadService : IUploadService
{
    private readonly IJSRuntime _js;
    private readonly HttpClient _http;
    public UploadService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<string> UploadImageAsync(IFileEntry file)
    {
        var content = new MultipartFormDataContent();

        // Đọc file từ IFileEntry bằng WriteToStreamAsync
        using var memoryStream = new MemoryStream();
        await file.WriteToStreamAsync(memoryStream);
        memoryStream.Position = 0;

        var fileContent = new StreamContent(memoryStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.Type);

        content.Add(fileContent, "file", file.Name);

        // Lấy token từ localStorage
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
        // Loại bỏ dấu ngoặc kép nếu có
        token = token.Trim('"');
        //xác thực token
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _http.PostAsync("api/upload/post-image", content);
        var result = await response.Content.ReadFromJsonAsync<UploadImageResponse>();

        return result?.Url ?? "";
    }
}
